using DataBase;
using DataBase.Models;
using PullOrder.Base;
using PullOrder.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PullOrderTransaction
{
    public class PullOrdersSStart : Base
    {
        private QueuePullOrderUserToken EbayUser;
        private List<Task> TaskList;
        private int MaxThreadCount;
        public PullOrdersSStart()
        {
            this.AppSettings = SetAppSettings();
        }

        public void Start()
        {
            TaskList = new List<Task>();
            MaxThreadCount = AppSettings.MaxThreadCount;
            while (true)
            {
                try
                {
                    CheckTaskIsCompleted();

                    if (MaxThreadCount == 0)
                    {
                        Thread.Sleep(2000);
                        continue;
                    }

                    using (mardevContext context = new mardevContext())
                    {
                        EbayUser = context.QueuePullOrderUserToken.Where(x => !x.IsUsed).FirstOrDefault();

                        if (EbayUser != null)
                        {
                            EbayUser.IsUsed = true;
                            context.QueuePullOrderUserToken.Update(EbayUser);
                            context.SaveChanges();
                        }
                    };
                    if (EbayUser == null || EbayUser.ApiUserPlatformTokenId == 0)
                    {
                        Thread.Sleep(10000);
                        continue;
                    }
                    Console.WriteLine("Get Login data at" + DateTime.Now.ToString("HH:mm:ss"));
                    PullOrderProcess DoPcc = new PullOrderProcess() { };
                    var NewTask = Task.Run(async () => { await DoPcc.Process(EbayUser); });
                    TaskList.Add(NewTask);
                    MaxThreadCount--;
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void CheckTaskIsCompleted()
        {
            if (TaskList.Count != 0)
            {
                //Task 在Running狀態無法跑foreach
                var GetNotRunList = TaskList.Where(x => x.Status != TaskStatus.Running).ToList();

                foreach (Task t in GetNotRunList)
                {
                    if (t.IsCompleted)
                    {
                        TaskList.Remove(t);
                        MaxThreadCount++;
                    }
                }
            }
        }
    }
}

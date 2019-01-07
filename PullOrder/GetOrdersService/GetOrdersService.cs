using ServiceReference1;
using System.Threading.Tasks;
using static ServiceReference1.eBayAPIInterfaceClient;

namespace EbayAPIService
{
    public class GetOrdersService
    {
        private eBayAPIInterfaceClient service;
        public GetOrdersService()
        {
            service = new eBayAPIInterfaceClient(EndpointConfiguration.eBayAPI);
        }

        public async Task GetOrdersFromEbay()
        {
            GetOrdersRequestType request = new GetOrdersRequestType()
            {

            };

            CustomSecurityHeaderType requesterCredentials = new CustomSecurityHeaderType
            {
                eBayAuthToken = "",
                Credentials = new UserIdPasswordType()
                {
                    //AppId = Loginitems.AppID,
                    //DevId = Loginitems.DevID,
                    //AuthCert = Loginitems.CertID
                }
            };
            GetOrdersResponse response = await service.GetOrdersAsync(requesterCredentials,request);
        }
    }
}

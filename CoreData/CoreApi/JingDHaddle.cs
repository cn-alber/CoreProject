using System.Collections.Generic;
using CoreModels;

namespace CoreDate.CoreApi
{
    public static class JingDHaddle{

        /// <summary>
		/// 获取token
		/// </summary>
        public static DataResult GetToken(string url, string grant_type, string code, string redirect_uri, string client_id, string client_secret){
            var result = new DataResult(1,null);
            
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", grant_type);
            parameters.Add("code", code);
            parameters.Add("redirect_uri", redirect_uri);
            parameters.Add("client_id", client_id);
            parameters.Add("client_secret", client_secret);
            var obj = new {
                grant_type = grant_type,
                code = code,
                redirect_uri = redirect_uri,
                client_id = client_id,
                client_secret = client_secret
            };
            var response = JsonResponse.CreatePostHttpResponse(url, parameters,obj);
            result.d = response.Result;
            
            return result;
        }












    }
}
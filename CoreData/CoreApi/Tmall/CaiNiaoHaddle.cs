using System;
using System.Collections.Generic;
using CoreModels;
using CoreModels.XyApi.Tmall;
using Newtonsoft.Json;

namespace CoreData.CoreApi
{
    public  class CaiNiaoHaddle:TmallBase
    {  

        #region 
        /// <summary>
        ///   参考网址： 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public static DataResult cntmsMailnoGet (){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.cntms.mailno.get");
                Tmparam.Add("session", TOKEN);
                
                CnTmsMailnoGetContentDomain content  = new CnTmsMailnoGetContentDomain();
                content.receiver_info = new CnTmsMailnoReceiverinfoDomain();
                content.receiver_info.receiver_province="江苏省";
                content.receiver_info.receiver_city="苏州市";
                content.receiver_info.receiver_area="常熟市";
                content.receiver_info.receiver_address = "梅李镇将泾村33号（226村道西150米瑞益纺织旁）";
                content.receiver_info.receiver_mobile = "13776218043";
                content.sender_info = new CnTmsMailnoSenderinfoDomain();
                content.sender_info.sender_province="江苏省";
                content.sender_info.sender_city = "苏州市";
                content.sender_info.sender_area="常熟市";
                content.sender_info.sender_address = "莫城管理区苏常公路戴家滨桥南，携云华东仓";
                content.sender_info.sender_name = "南极人";
                content.sender_info.sender_mobile = "15151434621";
                var item = new CnTmsMailnoItemDomain();
                item.item_name = "南极人2016冬季休闲男士毛领连帽保暖羽绒服中长款修身加厚外套男";
                item.item_qty = 1;
                content.items = new List<CnTmsMailnoItemDomain>();
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                content.items.Add(item);                                
                content.order_source = "TM";
                content.solutions_code="DM092876";
                content.trade_id = "2605837640524916";
                
                Console.WriteLine("-----------------");
                Console.WriteLine(JsonConvert.SerializeObject(content));
                Console.WriteLine("-----------------");

                Tmparam.Add("content",JsonConvert.SerializeObject(content).Replace("\"","\'"));// 
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam(); 
            }        
            return result;
        }
        #endregion


        #region 
        /// <summary>
        ///   参考网址： 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public static DataResult wlbWaybillIGet(string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.waybill.ii.get ");
                Tmparam.Add("session", token);

                WaybillApplyNewRequest waybill = new WaybillApplyNewRequest();
                waybill.cp_code = "YTO";
                
                waybill.shipping_address = new WaybillAddress();
                waybill.shipping_address.province = "江苏省";
                waybill.shipping_address.address_detail = "莫城管理区苏常公路戴家滨桥南，携云华东仓";

                waybill.trade_order_info_cols = new List<TradeOrderInfo>();
                    TradeOrderInfo order = new TradeOrderInfo();
                    order.consignee_name = "陈杰";
                    order.order_channels_type = "TM";
                    order.trade_order_list = new List<string>(){"2605837640524916"};
                    order.consignee_phone = "13776218043";
                    order.consignee_address = new WaybillAddress();
                    order.consignee_address.province="江苏省";
                    order.consignee_address.address_detail = "梅李镇将泾村33号（226村道西150米瑞益纺织旁）";
                    order.package_items = new List<PackageItem>();
                        PackageItem item = new PackageItem();
                        item.item_name = "南极人2016冬季休闲男士毛领连帽保暖羽绒服中长款修身加厚外套男";   
                        item.count = 1;
                    order.package_items.Add(item);
                    order.product_type = "STANDARD_EXPRESS";
                      


                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res;
                }            
            }catch(Exception ex){                
                result.s = -1;
                result.d =  ex.Message;
            }finally{
                cleanParam();
            }        
            return result;
        }
        #endregion










    
    }
    
}
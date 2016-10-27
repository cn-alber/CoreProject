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
        ///  菜鸟电子面单的云打印申请电子面单号的方法 参考网址： http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.WkHGkn&treeId=17&articleId=26869&docType=2
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public static DataResult WaybillIIGet(WaybillCloudPrintApplyNewRequest waybill){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.waybill.ii.get");
                Tmparam.Add("session", TOKEN);

                // WaybillCloudPrintApplyNewRequest waybill = new WaybillCloudPrintApplyNewRequest();
                // waybill.cp_code = "YTO";
                // waybill.sender = new UserInfoDto{
                //     address = new AddressDto{
                //         detail = "莫城环来泾路南极云商仓库部",
                //         city = "苏州市",
                //         district="常熟市",
                //         province = "江苏省"
                //     },
                //     name="南极人",
                //     mobile = "15151434621" 
                // };
                // waybill.trade_order_info_dtos = new List<TradeOrderInfoDto>();
                //    TradeOrderInfoDto order = new TradeOrderInfoDto();
                //    order.object_id = 1;
                //    order.order_info = new OrderInfoDto{
                //        order_channels_type = "TM",
                //        trade_order_list = new List<string>(){"2605837640524916"}
                //    };
                 
                //    order.package_info = new PackageInfoDto();
                //    order.package_info.items = new List<Item>();
                //         Item item = new Item();
                //         item.count = 1;
                //         item.name = "南极人2016冬季休闲男士毛领连帽保暖羽绒服中长款修身加厚外套男";
                //    order.package_info.items.Add(item);
                //    order.package_info.volume = 1;
                 
                //    order.recipient=new UserInfoDto{
                //        address = new AddressDto{
                //            detail = "梅李镇将泾村33号（226村道西150米瑞益纺织旁）",
                //            city = "苏州市",
                //         district="常熟市",
                //            province = "江苏省"
                //        },
                //        name = "陈杰",
                //        mobile = "13776218043"
                       
                //    };
                //    order.template_url = "http://cloudprint.cainiao.com/template/standard/101/524";
                //    order.user_id = 2058964557;
                // waybill.trade_order_info_dtos.Add(order);

                Tmparam.Add("param_waybill_cloud_print_apply_new_request",JsonConvert.SerializeObject(waybill).Replace("\"","\'"));
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;                     
                }else{
                    result.d = res.cainiao_waybill_ii_get_response.modules.waybill_cloud_print_response;
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
        ///  通过面单号查询电子面单信息  参考网址： http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.gSnxce&treeId=17&articleId=104859&docType=1
        /// </summary>
        public static DataResult waybillIIQueryByCode (List<WaybillDetailQueryByWaybillCodeRequest> waybill){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.waybill.ii.query.by.waybillcode");
                Tmparam.Add("session", TOKEN);

                Tmparam.Add("param_list",JsonConvert.SerializeObject(waybill).Replace("\"","\'"));
                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;                    
                }else{
                    result.d = res.cainiao_waybill_ii_query_by_waybillcode_response.modules.waybill_cloud_print_with_result_desc_response;
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
        ///   电子面单云打印更新接口 参考网址： http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.XVv8Co&treeId=17&articleId=26870&docType=2
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public static DataResult waybillIIUpdate(WaybillCloudPrintUpdateRequest waybill){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.waybill.ii.update");
                Tmparam.Add("session", TOKEN);

                Tmparam.Add("param_list",JsonConvert.SerializeObject(waybill).Replace("\"","\'"));
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
        public static DataResult demo(string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "");
                Tmparam.Add("session", TOKEN);


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
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
                if(response.Result.ToString().IndexOf("error_response") > 0){
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
                if(response.Result.ToString().IndexOf("error_response") > 0){
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
                if(response.Result.ToString().IndexOf("error_response") > 0){
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
        ///  商家查询物流商产品类型接口 参考网址： http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.HRph85&treeId=17&articleId=26765&docType=2
        /// </summary>
        public static DataResult waybillIIProduct(string cp_code,string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.waybill.ii.product");
                Tmparam.Add("session", token);
                Tmparam.Add("cp_code", cp_code);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.cainiao_waybill_ii_product_response.product_types.waybill_product_type;
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
        ///  查询面单服务订购及面单使用情况 参考网址：http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.cV88gv&treeId=17&articleId=27125&docType=2 
        /// </summary>
        public static DataResult waybillIISearch(string cp_code,string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.waybill.ii.search");
                Tmparam.Add("session", token);
                Tmparam.Add("cp_code", cp_code);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.cainiao_waybill_ii_search_response.waybill_apply_subscription_cols.waybill_apply_subscription_info;
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
        ///   商家取消获取的电子面单号 参考网址： http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.tmwLZu&treeId=17&articleId=26766&docType=2
        /// </summary>
        public static DataResult waybillIICancel (string cp_code,string waybill_code,string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "");
                Tmparam.Add("session", token);
                Tmparam.Add("cp_code", cp_code);
                Tmparam.Add("waybill_code", waybill_code);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
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
        ///  获取所有的菜鸟标准电子面单模板 参考网址： http://open.taobao.com/doc2/apiDetail.htm?apiId=26756&scopeId=12040
        /// </summary>
        public static DataResult cloudTempGet(string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.cloudprint.stdtemplates.get");
                Tmparam.Add("session", token);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
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
        ///  获取用户使用的菜鸟电子面单模板信息 参考网址：http://open.taobao.com/docs/api.htm?spm=a219a.7395905.0.0.Z6PPxX&scopeId=12040&apiId=26758
        /// </summary>
        public static DataResult cloudMyTempGet(string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.cloudprint.mystdtemplates.get");
                Tmparam.Add("session", token);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.cainiao_cloudprint_mystdtemplates_get_response.result.datas;
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
        ///  获取商家的自定义区模板信息 参考网址： http://open.taobao.com/docs/api.htm?spm=a219a.7395905.0.0.TUW9MR&scopeId=12040&apiId=26800
        /// </summary>
        public static DataResult cloudCustomGet(string token,string template_id){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.cloudprint.customares.get");
                Tmparam.Add("session", token);
                Tmparam.Add("template_id", template_id);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.cainiao_cloudprint_customares_get_response.result.datas;
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
        public static DataResult cloudIsvTempGet(string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.cloudprint.isvtemplates.get ");
                Tmparam.Add("session", token);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.cainiao_cloudprint_isvtemplates_get_response.result.datas;
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
        /// 云打印模板迁移接口  参考网址： http://open.taobao.com/docs/api.htm?spm=a219a.7395905.0.0.xnlXeb&scopeId=12040&apiId=27500
        /// </summary>
        public static DataResult cloudTempMigrate(string token,string tempalte_id,string custom_area_name,string custom_area_content){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.cloudprint.templates.migrate");
                Tmparam.Add("session", TOKEN);

                Tmparam.Add("tempalte_id", tempalte_id);
                Tmparam.Add("custom_area_name", custom_area_name);
                Tmparam.Add("custom_area_content", custom_area_content);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.cainiao_cloudprint_templates_migrate_response.result.data;
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
        ///  客户端更新回调 参考网址： http://open.taobao.com/docs/api.htm?spm=a219a.7395905.0.0.GIrUzc&scopeId=12040&apiId=27776
        /// </summary>
        public static DataResult clientupdate(string token,string mac,string version,string update_typa_name){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "cainiao.waybillprint.clientupdate.callback");
                Tmparam.Add("session", token);

                Tmparam.Add("mac", mac);
                Tmparam.Add("version", version);
                Tmparam.Add("update_typa_name", update_typa_name);

                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
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
        public static DataResult demo(string token){
            var result = new DataResult(1,null);
            try{                                        
                Tmparam.Add("method", "");
                Tmparam.Add("session", TOKEN);


                string sign = JsonResponse.SignTopRequest(Tmparam, SECRET, "md5");
                Tmparam.Add("sign", sign);//                                      
                var response = JsonResponse.CreatePostHttpResponse(SERVER_URL, Tmparam);            
                var res = JsonConvert.DeserializeObject<dynamic>(response.Result.ToString().Replace("\"","\'")+"}");                                                               
                if(response.Result.ToString().IndexOf("error_response") > 0){
                    result.s = -1;
                    result.d ="code:"+res.error_response.code+" "+res.error_response.sub_msg+" "+res.error_response.msg;
                }else{
                    result.d = res.cainiao_cloudprint_stdtemplates_get_response.result.datas.standard_template_result;
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
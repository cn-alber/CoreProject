接口文件：CoreWebApi/Controllers/Api/Tmall/TmTtradesControllers.cs

function说明
==============
1.soldget()            下载订单

 路由:`/core/Api/TmTrades/soldget`
 
 参数：
```sh
    input: 
            {
                fields					 必选，需要返回的字段列表，多个字段用半角逗号分隔，可选值为返回示例中能看到的所有字段						
                start_created	         查询三个月内交易创建时间开始。格式:yyyy-MM-dd HH:mm:ss													
                end_created	             查询交易创建时间结束。格式:yyyy-MM-dd HH:mm:ss															
                status			         交易状态																									
                buyer_nick		         买家昵称																									
                type			         交易类型列表																								
                ext_type		         扩展类型																									
                rate_status	             评价状态 																									
                tag			             卖家对交易的自定义分组标签																				
                page		             页码																										
                pageSize		         每页条数																									
                use_has_next	         是否启用has_next的分页方式，如果指定true,则返回的结果中不包含总记录数，但是会新增一个是否存在下一页的的字段
            }
    output:
            s: 1 || error_code
            d:[   {
                    "trades_sold_get_response": {
                    "total_results": 43,
                    "trades": {
                    "trade": [
                        {
                        "adjust_fee": "0.00",
                        "buyer_nick": "sandbox_cilai_c",
                        "buyer_obtain_point_fee": 0,
                        "buyer_rate": false,
                        "cod_fee": "0.00",
                        "cod_status": "NEW_CREATED",
                        "created": "2016-10-20 13:27:13",
                        "discount_fee": "0.00",
                        "modified": "2016-10-20 13:27:49",
                        "num": 5,
                        "num_iid": 2100710399384,
                        "orders": {
                            "order": [
                            {
                            "adjust_fee": "0.00",
                            "buyer_rate": false,
                            "discount_fee": "0.00",
                            "num": 5,
                            "num_iid": 2100710399384,
                            "oid": 194145147111084,
                            "payment": "510.00",
                            "pic_path": "http://img03.daily.taobao.net/bao/uploaded/i3/TB10dTHXXXXXXawXXXXXXXXXXXX_!!0-item_pic.jpg",
                            "price": "100.00",
                            "refund_status": "NO_REFUND",
                            "seller_rate": false,
                            "seller_type": "C",
                            "status": "WAIT_SELLER_SEND_GOODS",
                            "title": "沙箱测试20150915001",
                            "total_fee": "500.00"
                            }
                        ]
                    },
                    "pay_time": "2016-10-20 13:27:49",
                    "payment": "510.00",
                    "pic_path": "http://img03.daily.taobao.net/bao/uploaded/i3/TB10dTHXXXXXXawXXXXXXXXXXXX_!!0-item_pic.jpg",
                    "point_fee": 0,
                    "post_fee": "10.00",
                    "price": "100.00",
                    "real_point_fee": 0,
                    "received_payment": "0.00",
                    "receiver_address": "大张楼镇淘宝测试发货",
                    "receiver_city": "济宁市",
                    "receiver_district": "嘉祥县",
                    "receiver_mobile": "12345678990",
                    "receiver_name": "淘宝测试发货",
                    "receiver_state": "山东省",
                    "receiver_zip": "272000",
                    "seller_nick": "sandbox_c_1",
                    "seller_rate": false,
                    "shipping_type": "express",
                    "sid": "194145147111084",
                    "status": "WAIT_SELLER_SEND_GOODS",
                    "tid": 194145147111084,
                    "title": "d[s50339501]",
                    "total_fee": "500.00",
                    "type": "fixed"
                    },
                    { },
                    { },
                ]
                },
                "request_id": "118g8gffrwskl"
                }
            }  ]
            m: 
    
```

2.oneget()            获取单笔交易的部分信息(性能高)

 路由:`/core/Api/TmTrades/oneget`
 
 参数：
```sh
    input: 
            {
                fields					 必选，需要返回的字段列表，多个字段用半角逗号分隔，可选值为返回示例中能看到的所有字段						
                tids                     必选， 交易ids,多条id 以 , 结尾
                token                    授权码
            }
    output:
                {
                    "s": 1,
                    "d": [
                        {
                        "adjust_fee": "0.00",
                        "buyer_nick": "sandbox_cilai_c",
                        "buyer_obtain_point_fee": 0,
                        "buyer_rate": false,
                        "cod_fee": "0.00",
                        "cod_status": "NEW_CREATED",
                        "commission_fee": "0.00",
                        "created": "2016-10-20 13:27:13",
                        "discount_fee": "0.00",
                        "modified": "2016-10-20 13:27:49",
                        "num": 5,
                        "num_iid": 2100710399384,
                        "orders": {
                            "order": [
                            {
                                "adjust_fee": "0.00",
                                "buyer_rate": false,
                                "discount_fee": "0.00",
                                "num": 5,
                                "num_iid": 2100710399384,
                                "oid": 194145147111084,
                                "payment": "510.00",
                                "pic_path": "http://img03.daily.taobao.net/bao/uploaded/i3/TB10dTHXXXXXXawXXXXXXXXXXXX_!!0-item_pic.jpg",
                                "price": "100.00",
                                "refund_status": "NO_REFUND",
                                "seller_rate": false,
                                "seller_type": "C",
                                "status": "WAIT_SELLER_SEND_GOODS",
                                "title": "沙箱测试20150915001",
                                "total_fee": "500.00"
                            }
                            ]
                        },
                        "pay_time": "2016-10-20 13:27:49",
                        "payment": "510.00",
                        "pic_path": "http://img03.daily.taobao.net/bao/uploaded/i3/TB10dTHXXXXXXawXXXXXXXXXXXX_!!0-item_pic.jpg",
                        "point_fee": 0,
                        "post_fee": "10.00",
                        "price": "100.00",
                        "real_point_fee": 0,
                        "received_payment": "0.00",
                        "seller_nick": "sandbox_c_1",
                        "seller_rate": false,
                        "shipping_type": "express",
                        "sid": "194145147111084",
                        "status": "WAIT_SELLER_SEND_GOODS",
                        "tid": 194145147111084,
                        "title": "d[s50339501]",
                        "total_fee": "500.00",
                        "type": "fixed"
                        }
                    ],
                    "m": ""
                    }
    
```
3.onlineSend()            在线订单发货处理（支持货到付款）

 路由:`/core/Api/TmSend/onlineSend`
 
 参数：
```sh
    input: 
            {
                token                    授权码
            }
```


4.onlineCancel()            取消物流订单接口

 路由:`/core/Api/TmSend/onlineCancel`
 
 参数：
```sh
    input: 
            {
                token                    授权码
            }
```

5.onlineConfirm()            确认发货通知接口

 路由:`/core/Api/TmSend/onlineConfirm`
 
 参数：
```sh
    input: 
            {
                token                    授权码
            }
```


6.offlineSend()            自己联系物流（线下物流）发货

 路由:`/core/Api/TmSend/offlineSend`
 
 参数：
```sh
    input: 
            {
                offlineSend offlinesend         线下发货Model
            }
```

7.dummySend()            虚拟发货

 路由:`/core/Api/TmSend/dummySend`
 
 参数：
```sh
    input: 
            {
                dummySend dummysend         虚拟发货Model
            }
```

8.orderCreateAndSend()            创建订单并发货

 路由:`/core/Api/TmSend/orderCreateAndSend`
 
 参数：
```sh
    input: 
            {
               
            }
```


9.ApplyGet()            查询买家申请的退款列表

 路由:`/core/Api/TmRefund/ApplyGet`
 
 参数：
```sh
    input: 
            {
                token
                fields
                status                  退款状态，默认查询所有退款状态的数据，除了默认值外每次只能查询一种状态
                seller_nick             卖家昵称
                type                    交易类型列表，一次查询多种类型可用半角逗号分隔，默认同时查询guarantee_trade, auto_delivery的2种类型的数据。
                page
                pageSize
            }
    output：
        {
        "s": 1,
        "d": {
            "refunds": {
            "refund": [
                {
                "buyer_nick": "sandbox_c_1",
                "created": "2016-09-23 14:13:28",
                "refund_fee": "99.00",
                "refund_id": 147802226992786,
                "seller_nick": "sandbox_cilai_c",
                "status": "WAIT_SELLER_CONFIRM_GOODS",
                "tid": 194164230048627,
                "title": "沙箱测试:自动添加商品9",
                "total_fee": "99.00"
                },
                {
                "buyer_nick": "sandbox_c_1",
                "created": "2016-08-04 12:00:25",
                "refund_fee": "1008.72",
                "refund_id": 147749821182786,
                "seller_nick": "sandbox_c_6",
                "status": "WAIT_SELLER_AGREE",
                "tid": 194336200338627,
                "title": "沙箱测试 zzz 2014品牌夏季韩版新款修身显瘦雪纺连衣裙",
                "total_fee": "1008.72"
                }]
            },
            "total_results": 460,
            "request_id": "118g8gg8w0z50"
        },
        "m": ""
        }

```
10.ReceiveGet()            查询卖家收到的退款列表

 路由:`/core/Api/TmRefund/ReceiveGet`
 
 参数：
```sh
    input: 
            {
                token
                fields
                status                  退款状态，默认查询所有退款状态的数据，除了默认值外每次只能查询一种状态
                buyer_nick              买家昵称
                type                    交易类型列表，一次查询多种类型可用半角逗号分隔，默认同时查询guarantee_trade, auto_delivery的2种类型的数据。
                page
                pageSize
            }
     output:
            {
                "s": 1,
                "d": {
                    "refunds": {
                    "refund": [
                        {
                        "buyer_nick": "sandbox_cilai_c",
                        "created": "2016-08-03 11:52:22",
                        "good_status": "BUYER_NOT_RECEIVED",
                        "has_good_return": false,
                        "modified": "2016-08-03 11:52:22",
                        "oid": 194174063521084,
                        "order_status": "WAIT_SELLER_SEND_GOODS",
                        "payment": "0.00",
                        "reason": "缺货",
                        "refund_fee": "110.00",
                        "refund_id": 40594600198410,
                        "refund_phase": "onsale",
                        "seller_nick": "sandbox_c_1",
                        "status": "WAIT_SELLER_AGREE",
                        "tid": 194174063521084,
                        "title": "沙箱测试20150915001",
                        "total_fee": "110.00"
                        },
                        {
                        "buyer_nick": "sandbox_c_12",
                        "created": "2016-06-01 15:09:40",
                        "desc": "部分退款2",
                        "good_status": "BUYER_NOT_RECEIVED",
                        "has_good_return": false,
                        "modified": "2016-06-01 15:09:40",
                        "oid": 194174061976510,
                        "order_status": "WAIT_SELLER_SEND_GOODS",
                        "payment": "0.00",
                        "reason": "协商一致退款",
                        "refund_fee": "252.77",
                        "refund_id": 146535512261065,
                        "refund_phase": "onsale",
                        "seller_nick": "sandbox_c_1",
                        "status": "WAIT_SELLER_AGREE",
                        "tid": 194174061956510,
                        "title": "沙箱测试商品by2048",
                        "total_fee": "252.77"
                        },]
                    },
                    "total_results": 608,
                    "request_id": "118g8ggcuahwt"
                },
                "m": ""
                }       
```


接口文件：CoreWebApi/Controllers/Api/Tmall/TmTtradesControllers.cs

function说明
==============
1.soldget()            下载订单

 路由:`/core/Api/TmTrades/soldget`
 
 参数：
```sh
    input: 
            {
                fields					 空即使用默认						
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
                            "sku_id": "31122967183",     // sku_Id 订单列表有时不会被显示
                            "sku_properties_name": "套餐种类:官方标配",
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

3.itempropsGet()            获取类目属性

 路由:`/core/Api/TmTrades/itempropsGet`
 
 参数：
```sh
    input: 
            {
                token                    授权码
                cid                      子类目ID
            }
     output：
            {
            "s": 1,
            "d": {
                "sku_props": [
                    {
                        "is_allow_alias": true,                 //是否允许别名
                        "is_color_prop": true,                  //是否颜色属性
                        "is_enum_prop": true,                   //是否枚举
                        "is_input_prop": true,                  //在is_enum_prop是true的前提下，是否是卖家可以自行输入
                        "is_item_prop": false,                  //是否商品属性
                        "is_key_prop": false,                   //是否关键属性
                        "is_material": false,                   //是否材质属性
                        "is_sale_prop": true,                   //是否销售属性
                        "material_do": {                        //材质详细
                            "materials": {}
                        },
                        "multi": true,                          //是否多选
                        "must": false,                          //是否必选
                        "name": "主要颜色",                      //属性名
                        "pid": 1627207,                         //属性ID
                        "prop_values": {
                            "prop_value": [
                                {
                                    "name": "乳白色",
                                    "vid": 28321                //属性值ID
                                },
                                {
                                    "name": "军绿色",
                                    "vid": 3232483
                                },
                                {
                                    "name": "卡其色",
                                    "vid": 28331
                                }                       
                            ]
                        },
                        "sort_order": 38,
                        "status": "normal",
                        "taosir_do": {
                                expr_el_list:{}                 //表达式元素list
                        }                         
                    },
                    {
                        "is_allow_alias": true,
                        "is_color_prop": false,
                        "is_enum_prop": true,
                        "is_input_prop": true,
                        "is_item_prop": false,
                        "is_key_prop": false,
                        "is_material": false,
                        "is_sale_prop": true,
                        "material_do": {
                            "materials": {}
                        },
                        "multi": true,
                        "must": false,
                        "name": "尺码",
                        "pid": 20509,
                        "prop_values": {
                            "prop_value": [
                                {
                                    "name": "145/80A",
                                    "vid": 649458002
                                },
                                {
                                    "name": "XXS",
                                    "vid": 28381
                                },
                                {
                                    "name": "150/80A",
                                    "vid": 66579689
                                },
                                {
                                    "name": "XS",
                                    "vid": 28313
                                }
                            ]
                        },
                        "sort_order": 40,
                        "status": "normal",
                        "taosir_do": {}
                    }
                ],
                "item_props": [            
                    {
                        "is_allow_alias": false,
                        "is_color_prop": false,
                        "is_enum_prop": true,
                        "is_input_prop": false,
                        "is_item_prop": true,
                        "is_key_prop": false,
                        "is_material": false,
                        "is_sale_prop": false,
                        "material_do": {
                            "materials": {}
                        },
                        "multi": false,
                        "must": false,
                        "name": "袖长",
                        "pid": 122216348,
                        "prop_values": {
                            "prop_value": [
                                {
                                    "name": "七分袖",
                                    "vid": 3216779
                                },
                                {
                                    "name": "九分袖",
                                    "vid": 11162412
                                },
                                {
                                    "name": "五分袖",
                                    "vid": 14587965
                                },
                                {
                                    "name": "无袖",
                                    "vid": 29446
                                },
                                {
                                    "name": "短袖",
                                    "vid": 29445
                                },
                                {
                                    "name": "长袖",
                                    "vid": 29444
                                }
                            ]
                        },
                        "sort_order": 0,
                        "status": "normal",
                        "taosir_do": {}
                    }
                ]
            },
            "m": ""
        }
```
3.sellercatsListGet()            获取自定义类目

 路由:`/core/Api/TmTrades/sellercatsListGet`
 
 参数：
```sh
    input: 
            {
                nick                     店铺昵称
                token                    授权码
            }
     output：
            {                
            "s": 1,
            "d": [
                {
                "cid": 1257873181,                  //卖家自定义类目编号
                "name": "2016秋季热卖",              //卖家自定义类目名称
                "parent_cid": 0,                    //父类目编号，值等于0：表示此类目为店铺下的一级类目，值不等于0：表示此类目有父类目
                "pic_url": "",                      //链接图片地址
                "sort_order": 1,                    //该类目在页面上的排序位置
                "type": "manual_type",              //店铺类目类型:可选值：manual_type：手动分类，new_type：新品上价， tree_type：二三级类目树 ，property_type：属性叶子类目树， brand_type：品牌推广
                "children": [
                    {
                    "cid": 1266533340,
                    "name": "秋羽绒",
                    "parent_cid": 1257873181,
                    "pic_url": "",
                    "sort_order": 1,
                    "type": "manual_type",
                    "children": null
                    },
                    {
                    "cid": 1266533341,
                    "name": "夹克",
                    "parent_cid": 1257873181,
                    "pic_url": "",
                    "sort_order": 2,
                    "type": "manual_type",
                    "children": null
                    }]
                    }],
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

11.ReceiveGet()            获取单笔退款详情

 路由:`/core/Api/TmRefund/OneGet`
 
 参数：
```sh
        input:c
            fields
            token
            refund_id       退款I单D
        
        output:
            {
            "s": 1,
            "d": {
                "refund": {
                "alipay_no": "2016060147475178",
                "attribute": ";refundFrom:2;a_n_c:serviceone;payMode:alipay;gaia:1;7d:1;a_i_c:10.189.224.91;reason:5;rootCat:50067548;newRefund:1;closeHint:1;returnFeeTo:1;lastOrder:0;shop_name:sandbox_c_1;",
                "buyer_nick": "sandbox_c_12",
                "created": "2016-06-01 15:09:40",
                "desc": "部分退款2",
                "good_status": "BUYER_NOT_RECEIVED",
                "has_good_return": false,
                "num": 3,
                "num_iid": 2100663189060,
                "oid": 194174061976510,
                "operation_contraint": "null",
                "payment": "0.00",
                "price": "90.60",
                "reason": "协商一致退款",
                "refund_fee": "252.77",
                "refund_id": 146535512261065,
                "refund_phase": "onsale",
                "refund_remind_timeout": {
                    "exist_timeout": true,
                    "remind_type": 1,
                    "timeout": "2016-06-03 15:09:40"
                },
                "refund_version": 1464764980000,
                "seller_nick": "sandbox_c_1",
                "status": "WAIT_SELLER_AGREE",
                "tid": 194174061956510,
                "title": "沙箱测试商品by2048",
                "total_fee": "252.77"
                },
                "request_id": "16ecr2nklz7h4"
            },
            "m": ""
            }

```

12.MessagesGet()            查询退款留言/凭证列表

 路由:`/core/Api/TmRefund/MessagesGet`
 
 参数：
```sh
        input:
            fields
            token
            refund_id       退款I单D
            refund_phase    退款阶段，天猫退款为必传。可选值：onsale（售中），aftersale（售后）
            page
            pageSize
        output:
            {
            "s": 1,
            "d": {
                "refund_messages_get_response": {
                "refund_messages": {
                "refund_message": [
                {
                "id": 5199969975,
                "message_type": "NORMAL"
                },
                {
                "id": 5200029393,
                "message_type": "NORMAL"
                },
                {
                "id": 5199643867,
                "message_type": "NORMAL"
                }
                ]
                },
                "total_results": 3,
                "request_id": "rxn9f5pq29oo"
                }
            },
            "m": ""
            }            


```

13.add()            创建订单并发货

 路由:`/core/Api/TmSku/Add`
 
 参数：
```sh
    input: 
        skuAddRequest{
                num_iid :               必选， 所属商品数字id，可通过 taobao.item.get 获取。必选
                properties :            必选， Sku属性串。格式:pid:vid;pid:vid,如:1627207:3232483;1630696:3284570,表示:机身颜色:军绿色;手机套餐:一电一充。
                quantity :              必选， Sku的库存数量。sku的总数量应该小于等于商品总数量(Item的NUM)。取值范围:大于零的整数
                price :                 必选， Sku的销售价格。商品的价格要在商品所有的sku的价格之间。精确到2位小数;单位:元。如:200.07，表示:200元7分
                outer_id :              Sku的商家外部id
                item_price :            sku所属商品的价格。当用户新增sku，使商品价格不属于sku价格之间的时候，用于修改商品的价格，使sku能够添加成功
                lang :                  Sku文字的版本。可选值:zh_HK(繁体),zh_CN(简体);默认值:zh_CN
                spec_id :               产品的规格信息
                sku_hd_length :         家装建材类目，商品SKU的长度，正整数，单位为cm，部分类目必选。天猫商家专用。 数据和SKU一一对应，用,分隔，如：20,30,30
                sku_hd_height :         家装建材类目，商品SKU的高度，单位为cm，部分类目必选。
                sku_hd_lamp_quantity :  家装建材类目，商品SKU的灯头数量，正整数，大于等于3，部分类目必选。天猫商家专用。 数据和SKU一一对应，用,分隔，如：3,5,7
                ignorewarning :         忽略警告提示.
        }
    output:

```


14.SkuGet()            获取SKU

 路由:`/core/Api/TmSku/Get`
 
 参数：
```sh
    input: 
        fields: 必选 
        sku_id: 必选， 
        num_iid: 商品的数字IID（num_iid和nick必传一个，推荐用num_iid），传商品的数字id返回的结果里包含cspu（SKu上的产品规格信息）。 nick: 卖家nick(num_iid和nick必传一个)，只传卖家nick时候，该api返回的结果不包含cspu（SKu上的产品规格信息）。
    output:
        {
        "s": 1,
        "d": {
            "created": "2015-09-20 10:29:23",
            "iid": "2100713359442",
            "modified": "2016-07-18 15:20:09",
            "num_iid": 2100713359442,
            "outer_id": "",
            "price": "999.00",
            "properties": "21684:6536025",
            "properties_name": "21684:6536025:套餐种类:官方标配",
            "quantity": 0,
            "sku_id": 31122967183,
            "status": "normal",
            "with_hold_quantity": 0
        },
        "m": ""
        }

```

15.Update()            更新SKU

 路由:`/core/Api/TmSku/Update`
 
 参数：
```sh
    input: 
        skuUpdateRequest
            {
                num_iid : 最小值：0 所属商品数字id，可通过 taobao.item.get 获取。必选
                properties : Sku属性串。格式:pid:vid;pid:vid,如:1627207:3232483;1630696:3284570,表示:机身颜色:军绿色;手机套餐:一电一充。
                quantity : Sku的库存数量。sku的总数量应该小于等于商品总数量(Item的NUM)。取值范围:大于零的整数
                price : Sku的销售价格。商品的价格要在商品所有的sku的价格之间。精确到2位小数;单位:元。如:200.07，表示:200元7分
                outer_id : Sku的商家外部id
                item_price : sku所属商品的价格。当用户新增sku，使商品价格不属于sku价格之间的时候，用于修改商品的价格，使sku能够添加成功
                lang : Sku文字的版本。可选值:zh_HK(繁体),zh_CN(简体);默认值:zh_CN
                spec_id : 产品的规格信息
                barcode : SKU条形码
                ignorewarning : 忽略警告提示.
            }
    output:

```
15.Update()            更新SKU

 路由:`/core/Api/TmSku/SkusGet`
 
 参数：
```sh
    input: 
        token
        fields
        num_iids      必选，sku所属商品数字id，必选。num_iid个数不能超过40个
    output:
        {
  "s": 1,
  "d": {
    "sku": [
      {
        "created": "2015-09-20 10:29:23",
        "iid": "2100713359442",
        "modified": "2016-07-18 15:20:09",
        "num_iid": 2100713359442,
        "outer_id": "",
        "price": "999.00",
        "properties": "21684:6536025",
        "properties_name": "21684:6536025:套餐种类:官方标配",
        "quantity": 0,
        "sku_id": 31122967183,
        "status": "normal"
      },
      { },
      { }
    ]
  },
  "m": ""
}
```

16.WaybillIIGet()            菜鸟电子面单的云打印申请电子面单号

 路由:`/core/Api/TmCaiNiao/WaybillIIGet`

 input：
    token                  授权码           
    其余参考： http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.WkHGkn&treeId=17&articleId=26869&docType=2
 output：
 ```sh
    {
    "s": 1,
    "d": {
         [{
            "object_id": "1",
            "print_data": "{'data':{'cpCode':'YTO','recipient':{'address':{'city':'苏州市','detail':'梅李镇将泾村33号（226村道西150米瑞益纺织旁）','district':'常熟市','province':'江苏省'},'mobile':'13776218043','name':'陈杰'},
                            'routingInfo':{'consolidation':{'code':'512102'},'origin':{'code':'512102','name':'江苏省苏州市常熟市'},'routeCode':'413-820 011','sortation':{'name':'常熟'}},
                            'sender':{'address':{'city':'苏州市','detail':'莫城环来泾路南极云商仓库部','district':'常熟市','province':'江苏省'},'mobile':'15151434621','name':'南极人'},
                            'shippingOption':{'code':'STANDARD_EXPRESS','title':'标准快递'},'waybillCode':'883190525934835804'},'signature':'MD:ACs6iwf4KHLCtd8ohW3lyg==','templateURL':'http://cloudprint.cainiao.com/template/standard/101/524'}",
            "waybill_code": "883190525934835804"
        },
        {},
        {}]    
    },
    "m": ""
    }

 ```

17.waybillIIQueryByCode()            通过面单号查询电子面单信息

 路由:`/core/Api/TmCaiNiao/waybillIIQueryByCode`

 input:
    token                  授权码           
    其余参考： http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.gSnxce&treeId=17&articleId=104859&docType=1
 output
```sh
{
  "s": 1,
  "d": {
         [
          {
            "success": true,
            "waybill_cloud_print_response": {
              "print_data": "{'data':{'cpCode':'YTO','recipient':{'address':{'city':'苏州市','detail':'梅李镇将泾村33号（226村道西150米瑞益纺织旁）','district':'常熟市','province':'江苏省'},'name':'陈杰','phone':'13776218043'},'routingInfo':{'consolidation':{'code':'512102','name':''},'origin':{'code':'512102','name':'江苏省苏州市常熟市'},'routeCode':'413-820 011','sortation':{'name':'常熟'}},'sender':{'address':{'city':'苏州市','detail':'莫城管理区苏常公路戴家滨桥南，携云华东仓，收件人：南极人  电话：18913668709','district':'常熟市','province':'江苏省'}},'shippingOption':{'code':'STANDARD_EXPRESS','title':'标准快递'},'waybillCode':'883162716003542975'},'signature':'MD:5thtqgb33Limln2kEl5O4A=='}",
              "waybill_code": "883162716003542975"
            }
          },
          { },
          { }
        ]
  },
  "m": ""
}

```

18.waybillIIUpdate()            更新电子面单

 路由:`/core/Api/TmCaiNiao/waybillIIUpdate`

 input:
    token                  授权码           
    其余参考： http://open.taobao.com/docs/doc.htm?spm=a219a.7629140.0.0.gSnxce&treeId=17&articleId=104859&docType=1
 output：  
```sh

```

19.waybillIIProduct()            商家查询物流商产品类型接口

 路由:`/core/Api/TmCaiNiao/waybillIIProduct`

 input:
    token                  授权码           
    cp_code                快递公司CODE
 output：  
```sh
    s:
    d:[
        {
            "code": "CAINIAO_4PL",
            "name": "圆通B网4PL",
            "service_types": {
            "waybill_service_type": [
                {
                "code": "YTO_B",
                "name": "同城配送"
                }
                ]
            }
        },
        {
            "code": "COD",
            "name": "代收货款",
            "service_types": {
            "waybill_service_type": [
                {
                "code": "SVC-COD",
                "name": "代收货款"
                }
                ]
            }
        },
            {
            "code": "STANDARD_EXPRESS",
            "name": "标准快递",
            "service_types": {}
            }
        ]

    m:


```

20.waybillIISearch()            查询面单服务订购及面单使用情况

 路由:`/core/Api/TmCaiNiao/waybillIISearch`

 input:
    token                  授权码           
    cp_code                快递公司CODE
 output：  
```sh
    s:
    d:[
    {
        "allocated_quantity": 137207,
        "branch_code": "512102",
        "branch_name": "江苏省苏州市常熟市",
        "cancel_quantity": 283,
        "print_quantity": 0,
        "quantity": 2816,
        "shipp_address_cols": {
        "address_dto": [{
                "city": "苏州市",
                "detail": "莫城管理区苏常公路戴家滨桥南，携云华东仓，收件人：南极人 电话：18913668709",
                "district": "常熟市",
                "province": "江苏省"
            },
            {
                "city": "苏州市",
                "detail": "莫城环来泾路南极云商仓储物流中心",
                "district": "常熟市",
                "province": "江苏省"
            },
            {
                "city": "苏州市",
                "detail": "莫城环来泾路南极云商仓库部",
                "district": "常熟市",
                "province": "江苏省"
            }
        ]}
    }
    ]

    m:
```

21.waybillIICancel()            商家取消获取的电子面单号

 路由:`/core/Api/TmCaiNiao/waybillIICancel`

 input:
    token                  授权码           
    cp_code                快递公司CODE
    waybill_code           电子面单号
 output：  
```sh

```


22.cloudTempGet()            获取云打印标准模板

 路由:`/core/Api/TmCaiNiao/cloudTempGet`

 input:
    token                  授权码           
 output：  
```sh
{
  "s": 1,
  "d": {
    "cainiao_cloudprint_stdtemplates_get_response": {
      "result": {
        "datas": {
          "standard_template_result": [
            {
              "cp_code": "ZJS",
              "standard_templates": {
                "standard_template_do": [
                  {
                    "standard_template_id": 901,
                    "standard_template_name": "宅急送标准模板",
                    "standard_template_url": "http://cloudprint.cainiao.com/template/standard/901/98"
                  },
                  {
                    "standard_template_id": 75104,
                    "standard_template_name": "宅急送标准三联模板",
                    "standard_template_url": "http://cloudprint.cainiao.com/template/standard/75104/3"
                  }
                ]
              }
            },
            {
              "cp_code": "YUNDA",
              "standard_templates": {
                "standard_template_do": [
                  {
                    "standard_template_id": 401,
                    "standard_template_name": "韵达快递标准模板",
                    "standard_template_url": "http://cloudprint.cainiao.com/template/standard/401/122"
                  },
                  {
                    "standard_template_id": 76303,
                    "standard_template_name": "韵达快递标准三联模板",
                    "standard_template_url": "http://cloudprint.cainiao.com/template/standard/76303/3"
                  }
                ]
              }
            }    
          ]
        },
        "error_code": "0",
        "success": true
      },
      "request_id": "uv7e5qn8pnp"
    }
  },
  "m": ""
}
```

23.cloudMyTempGet()            获取用户使用的菜鸟电子面单模板信息

 路由:`/core/Api/TmCaiNiao/cloudMyTempGet`

 input:
    token                  授权码           
 output：  
```sh

```


24.cloudCustomGet()            获取商家的自定义区模板信息

 路由:`/core/Api/TmCaiNiao/cloudCustomGet`

 input:
    token                  授权码      
    template_id            用户使用的标准模板ID     
 output：  
```sh

```

25.cloudTempMigrate()            获取商家的自定义区模板信息

 路由:`/core/Api/TmCaiNiao/cloudTempMigrate`

 input:
    token                  授权码      
    template_id            用户使用的标准模板ID
    custom_area_name       自定义区名称
    custom_area_content    自定义区内容     
 output：  
```sh
    {
    "s": 1,
    "d": {       
            "custom_area_id": 504108,
            "custom_area_url": "http://cloudprint.cainiao.com/template/customArea/504108",
            "keys": {},
            "standard_template_id": 101,
            "standard_template_url": "http://cloudprint.cainiao.com/template/standard/101",
            "user_template_id": 186601           
        },
    "m": ""
    }
```

26.clientUpdate()            客户端更新回调

 路由:`/core/Api/TmCaiNiao/clientUpdate`

 input:
    token                  授权码      
    mac                    客户端mac
    version                最新的、需要更新的版本
    update_typa_name       更新类型     
 output：  
```sh

```

27.onsaleGet()            获取店铺在售商品存入本地数据库

 路由:`/core/Api/TmItem/onsaleGet`

 input:
        page
        pageSize
        start_modified
        end_modified

28.sellerGet()            获取下载店铺在售商品sku信息

 路由:`/core/Api/TmItem/sellerGet`

 input:
        none


























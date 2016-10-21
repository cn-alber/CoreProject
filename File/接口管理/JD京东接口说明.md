接口文件：CoreWebApi/Controllers/Api/JingDong/JdOrderControllers.cs

function说明
==============
1.orderDownload()            下载订单

 路由:`/core/Api/JdOrder/download`
 
 参数：
```sh
    input: 
            {
               token         授权码
               start_date    WAIT_SELLER_STOCK_OUT 等待出库，则start_date可以为否（开始时间和结束时间均为空，默认返回前一个月的订单），order_state为其他值，则start_date必须为是（开始时间和结束时间，不得相差超过1个月。此时间仅针对订单状态及运单号修改的时间） 
               end_date      WAIT_SELLER_STOCK_OUT 等待出库，则start_date可以为否（开始时间和结束时间均为空，默认返回前一个月的订单），order_state为其他值，则start_date必须为是（开始时间和结束时间，不得相差超过1个月。此时间仅针对订单状态及运单号修改的时间） 
               order_state   多订单状态可以用英文逗号隔开 1）WAIT_SELLER_STOCK_OUT 等待出库 2）SEND_TO_DISTRIBUTION_CENER 发往配送中心（只适用于LBP，SOPL商家） 3）DISTRIBUTION_CENTER_RECEIVED 配送中心已收货（只适用于LBP，SOPL商家） 4）WAIT_GOODS_RECEIVE_CONFIRM 等待确认收货 5）RECEIPTS_CONFIRM 收款确认（服务完成）（只适用于LBP，SOPL商家） 6）WAIT_SELLER_DELIVERY等待发货（只适用于海外购商家，等待境内发货 标签下的订单） 7）FINISHED_L 完成 8）TRADE_CANCELED 取消 9）LOCKED 已锁定 10）PAUSE 暂停
               page
               pageSize      每页的条数（最大page_size 100条）
            }
    output:
            s: 1 || error_code
            d:[{
                 [  order_total：100
                    order_info_list{
                        "order_info": {
                              "modified": "2016-07-17 13:23:33",     修改时间
                              "customs": "",
                              "order_id": "20229596637",      订单id
                              "vender_id": "209379",          供应商id
                              "pay_type": "4-在线支付",		支付方式
                              "order_total_price": "89.00",   订单总价
                              "order_seller_price": "89.00",  订单售价
                              "order_payment": "89.00",       实际付款
                              "freight_price": "0.00",        运费
                              "seller_discount": "0.00",      折扣  
                              "order_state": "FINISHED_L",    订单状态
                              "delivery_type": "任意时间",    交货类型（百度翻译的...）
                              "invoice_info": "不需要开具发票", 发票信息
                              "order_remark": "",               订单备注
                              "order_start_time": "2016-07-15 22:39:27",  订单创建时间
                              "consignee_info": {               买家信息
                                    "fullname": "杨军",			
                                    "telephone": "13956922333",
                                    "mobile": "13956922333",
                                    "province": "安徽",
                                    "city": "合肥市",
                                    "county": "肥东县",
                                    "full_address": "安徽合肥市肥东县城区琪瑞大酒店-老汽车站"
                              },
                              "item_info_list": [               购买清单（数组类型）
                                    {
                                    "sku_id": "10268952835",
                                    "outer_sku_id": "DC2TB800C048185",
                                    "sku_name": "2016雅鹿/YALU  UOMO男装翻领短袖T恤男夏季男士纯棉polo衫休闲t恤 深紫色 185",
                                    "jd_price": "89.00",
                                    "gift_point": "0",
                                    "ware_id": "10052549317",
                                    "item_total": "1"
                                    }
                              ],
                              "coupon_detail_list": [          优惠券（数组）
                                    {
                                    "order_id": "",
                                    "sku_id": "",
                                    "coupon_type": "",
                                    "coupon_price": ""
                                    }
                              ],
                              "order_type": "22",              订单类型
                              "order_source": "微信订单",         订单来源
                              "store_order": "京仓订单",         
                              "customs_model": ""
                        }
                    },
                    {
                       "order_info"：{}   
                    }
                ]
            }]
            m: 
    
```


2.downByIds()            下载订单

 路由:`/core/Api/JdOrder/downByIds`
 
 参数：
```sh
    input: 
            {
               token                      授权码
               order_ids                  订单ID数组（以 ,连接，最大值100 ）
               optional_fields            访问字段，不填则全显
               order_state                订单状态
            }
    output:
            s: 1 || error_code
            d:[{ 
                 order_total：100
                 order_info_list{
                          order_info{同上},
                          order_info{},
                  }  }]
            m:  
    
```

3.RefundList()            退款单列表获取

 路由:`/core/Api/JdRefund/RefundList`
 
 参数：
```sh
    input: 
            {
                  ids					可为空，批量传入退款单id，格式为'id,id'，传入id数不能超过pageSize 								
                  status				可为空，退款申请单状态 0、未审核 1、审核通过2、审核不通过 3、京东财务审核通过 4、京东财务审核不通过 5、人工审核通过 6、拦截并退款 7、青龙拦截成功 8、青龙拦截失败 9、强制关单并退款。不传是查询全部状态 		
                  orderId				可为空，订单id 
                  buyerId				可为空，客户帐号 
                  buyerName				可为空，客户姓名 
                  applyTimeStart			可为空，申请日期（start）格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）
                  applyTimeEnd			可为空，申请日期（end）格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）
                  checkTimeStart			可为空，审核日期(start) 格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07） 
                  checkTimeEnd			可为空，审核日期(END) 格式类型（yyyy-MM-dd hh:mm:ss,2013-08-06 21:02:07）
                  pageIndex				页码(显示多少页，区间为1-1000)
                  pageSize				每页显示多少条（区间为1-50）

            }
    output:
            {
                  "s": 1,
                  "d": {
                        "result": [
                              {
                                    "applyRefundSum": 0,
                                    "applyTime": "2016-10-21 10:37:13",
                                    "buyerId": "13643169826_p",
                                    "buyerName": "张艳菊",
                                    "id": 1000399977,
                                    "orderId": "40204951366",
                                    "status": 0
                              },
                              {
                                    "applyRefundSum": 0,
                                    "applyTime": "2016-10-21 09:49:07",
                                    "buyerId": "jd_7cfe417d43793",
                                    "buyerName": "刘淑贤",
                                    "id": 1000196441,
                                    "orderId": "40617539947",
                                    "status": 0
                              }
                        ],
                        "success": true,
                        "totalCount": 265                            总数
                  },
                  "m": ""
            }
    
```


4.RefundById()            获取单条退款单

 路由:`/core/Api/JdRefund/RefundById`
 
 参数：
```sh
    input: 
            {
                  id		    退款单id	
                  token           授权码							
            }
    output:
            {
            "s": 1,
            "d": [
                  {
                        "applyRefundSum": 0,
                        "applyTime": "2016-10-21 10:37:13",
                        "buyerId": "13643169826_p",
                        "buyerName": "张艳菊",
                        "id": 1000399977,
                        "orderId": "40204951366",
                        "status": 0
                  }
            ],
            "m": ""
            }
    
```

5.ReplyRefund()            商家审核退款单

 路由:`/core/Api/JdRefund/ReplyRefund`
 
 参数：
```sh
   input: 
            token           授权码
```


6.GetWaitRefundNum()            待处理退款单数查询

 路由:`/core/Api/JdRefund/GetWaitRefundNum`
 
 参数：
```sh
   input: 
            token           授权码
    output:
            {
              "s": 1,
              "d": 2,   //待处理数
              "m": ""
            }

```

7.SkuAdd()            增加SKU信息

 路由:`/core/Api/JdSku/SkuAdd`
 
 参数：
```sh
      input:
            token           授权码
```

8.SkuUpdate()            修改SKU库存信息

 路由:`/core/Api/JdSku/SkuUpdate`
 
 参数：
```sh
      input:
            token           授权码
```

9.SkuDelete()            删除Sku信息

 路由:`/core/Api/JdSku/SkuDelete`
 
 参数：
```sh
      input:
            token           授权码
```

10.CustomGet()            根据外部ID获取商品SKU

 路由:`/core/Api/JdSku/CustomGet`
 
 参数：
```sh
      input:
            outer_id        外部ID
            token           授权码
      output:
            {
            "s": 1,
            "d": {
                  "status": "INVALID",
                  "attributes": "1000012739:1555355141^1000012740:1555354653",
                  "created": "2016-05-10 04:09:32",
                  "modified": "2016-06-04 18:14:37",
                  "sku_id": 10268951353,
                  "ware_id": 10052545379,
                  "shop_id": 200753,
                  "jd_price": "89.00",
                  "cost_price": "0.00",
                  "market_price": "228.00",
                  "stock_num": 20,
                  "outer_id": "DC2TB800C048185",
                  "color_value": "紫色",
                  "size_value": "185",
                  "sku_upccode": ""
            },
            "m": ""
            }

```

11.SkusGet()            根据商品ID列表获取商品SKU信息

 路由:`/core/Api/JdSku/SkusGet`
 
 参数：
```sh
      input:
            ware_ids        sku所属商品id，必选。ware_ids个数不能超过10个
            token           授权码
      output:
            {
            "s": 1,
            "d":[ {
                  "status": "INVALID",
                  "attributes": "1000012739:1555355141^1000012740:1555354653",
                  "created": "2016-05-10 04:09:32",
                  "modified": "2016-06-04 18:14:37",
                  "sku_id": 10268951353,
                  "ware_id": 10052545379,
                  "shop_id": 200753,
                  "jd_price": "89.00",
                  "cost_price": "0.00",
                  "market_price": "228.00",
                  "stock_num": 20,
                  "outer_id": "DC2TB800C048185",
                  "color_value": "紫色",
                  "size_value": "185",
                  "sku_upccode": ""
            },
             {
                  "status": "INVALID",
                  "attributes": "1000012739:1555356041^1000012740:1555351986",
                  "created": "2016-05-09 20:09:32",
                  "modified": "2016-06-04 10:14:37",
                  "sku_id": 10268951345,
                  "ware_id": 10052545379,
                  "shop_id": 200753,
                  "jd_price": "89.00",
                  "cost_price": "0.00",
                  "market_price": "228.00",
                  "stock_num": 20,
                  "outer_id": "DC2TB800C068175",
                  "color_value": "浅蓝色",
                  "size_value": "175",
                  "sku_upccode": ""
            }]
            "m": ""
            }

```

12.SkuGet()            获取单个Sku信息

 路由:`/core/Api/JdSku/SkuGet`
 
 参数：
```sh
      input:
            sku_id          SKU ID
            token           授权码
      output:
            {
            "s": 1,
            "d": {
                  "status": "INVALID",
                  "attributes": "1000012739:1555355141^1000012740:1555354653",
                  "created": "2016-05-10 04:09:32",
                  "modified": "2016-06-04 18:14:37",
                  "sku_id": 10268951353,
                  "ware_id": 10052545379,
                  "shop_id": 200753,
                  "jd_price": "89.00",
                  "cost_price": "0.00",
                  "market_price": "228.00",
                  "stock_num": 20,
                  "outer_id": "DC2TB800C048185",
                  "color_value": "紫色",
                  "size_value": "185",
                  "sku_upccode": ""
            },
            "m": ""
            }

```
12.ListingGet()            获取商品上架的商品信息

 路由:`/core/Api/JdSku/ListingGet`
 
 参数：
```sh
      input:
            start_modified          起始席间
            end_modified            结束时间
            token                   授权码
            cid                     类目ID（可为空）
            page
            page_size
            
      output:
            {
            "s": 1,
            "d": [
                  {
                        "title": "雅鹿2016秋季新款男士棒球领时尚休闲夹克 外套男潮",
                        "attributes": "124240:679561^15469:258395^15462:207762^10123169:10674595^15467:256506^89095:564657^15464:256478^10123167:10674582^15465:256490^1000012733:1556620976^89096:564704^89094:611425^118517:656398^15471:256537^118516:656373^15466:564640^15473:256564^15501:260230^105437:613888^1000012734:1556622848,1556623828,1556621581,1556626108,1556622470^15468:258386^118514:656360",
                        "logo": "http://img10.360buyimg.com/n0/jfs/t3208/234/981586264/186701/bec71fae/57c3e58bNf6938a62.jpg",
                        "creator": "",
                        "status": "VALID",
                        "weight": "0.5",
                        "created": "2016-08-29 15:35:37",
                        "modified": "2016-09-27 17:51:54",
                        "ware_id": 10125211072,
                        "spu_id": 0,
                        "cid": 9730,
                        "vender_id": 209379,
                        "shop_id": 200753,
                        "ware_status": "ON_SALE",
                        "item_num": "DC3JA003S",
                        "upc_code": "",
                        "transport_id": 0,
                        "online_time": "2016-09-09 14:57:36",
                        "offline_time": "2016-08-29 15:35:37",
                        "cost_price": "0.00",
                        "market_price": "529.00",
                        "jd_price": "529.00",
                        "stock_num": 21
                  },
                  {
                        "title": "雅鹿2016秋季新款时尚小立领休闲夹克男",
                        "attributes": "89094:611425^15468:256521^15501:260230^15465:256482^89095:564657^15462:207762^89096:564704^124240:679561^15469:258395^1000012734:1556622848,1556623828,1556621581,1556626108,1556622470^15471:256545^10123167:10674581^105437:613888^118514:679556^118517:656398^15464:256478^15473:256564^1000012733:1556625708,1556621579^15466:256495^15467:256506^118516:656383^10123169:10674593",
                        "logo": "http://img10.360buyimg.com/n0/jfs/t3046/270/993939413/186857/6e2b5dbe/57c3db9eNa925b684.jpg",
                        "creator": "",
                        "status": "VALID",
                        "weight": "0.5",
                        "created": "2016-08-29 14:54:56",
                        "modified": "2016-09-27 17:51:57",
                        "ware_id": 10125192581,
                        "spu_id": 0,
                        "cid": 9730,
                        "vender_id": 209379,
                        "shop_id": 200753,
                        "ware_status": "ON_SALE",
                        "item_num": "DC3JA002S",
                        "upc_code": "",
                        "transport_id": 0,
                        "online_time": "2016-09-09 14:57:36",
                        "offline_time": "2016-08-29 14:54:55",
                        "cost_price": "0.00",
                        "market_price": "369.00",
                        "jd_price": "369.00",
                        "stock_num": 30
                  }
                  ],
            "m": ""
            }

```

12.DelistingGet()            获取商品下架的商品信息

 路由:`/core/Api/JdSku/DelistingGet`
 
 参数：
```sh
      input:
            start_modified          起始席间
            end_modified            结束时间
            token                   授权码
            cid                     类目ID（可为空）
            page
            page_size
            
      output:
            {
            "s": 1,
            "d": [
                  {
                        "title": "雅鹿2016秋季新款男士棒球领时尚休闲夹克 外套男潮",
                        "attributes": "124240:679561^15469:258395^15462:207762^10123169:10674595^15467:256506^89095:564657^15464:256478^10123167:10674582^15465:256490^1000012733:1556620976^89096:564704^89094:611425^118517:656398^15471:256537^118516:656373^15466:564640^15473:256564^15501:260230^105437:613888^1000012734:1556622848,1556623828,1556621581,1556626108,1556622470^15468:258386^118514:656360",
                        "logo": "http://img10.360buyimg.com/n0/jfs/t3208/234/981586264/186701/bec71fae/57c3e58bNf6938a62.jpg",
                        "creator": "",
                        "status": "VALID",
                        "weight": "0.5",
                        "created": "2016-08-29 15:35:37",
                        "modified": "2016-09-27 17:51:54",
                        "ware_id": 10125211072,
                        "spu_id": 0,
                        "cid": 9730,
                        "vender_id": 209379,
                        "shop_id": 200753,
                        "ware_status": "ON_SALE",
                        "item_num": "DC3JA003S",
                        "upc_code": "",
                        "transport_id": 0,
                        "online_time": "2016-09-09 14:57:36",
                        "offline_time": "2016-08-29 15:35:37",
                        "cost_price": "0.00",
                        "market_price": "529.00",
                        "jd_price": "529.00",
                        "stock_num": 21
                  },
                  {
                        "title": "雅鹿2016秋季新款时尚小立领休闲夹克男",
                        "attributes": "89094:611425^15468:256521^15501:260230^15465:256482^89095:564657^15462:207762^89096:564704^124240:679561^15469:258395^1000012734:1556622848,1556623828,1556621581,1556626108,1556622470^15471:256545^10123167:10674581^105437:613888^118514:679556^118517:656398^15464:256478^15473:256564^1000012733:1556625708,1556621579^15466:256495^15467:256506^118516:656383^10123169:10674593",
                        "logo": "http://img10.360buyimg.com/n0/jfs/t3046/270/993939413/186857/6e2b5dbe/57c3db9eNa925b684.jpg",
                        "creator": "",
                        "status": "VALID",
                        "weight": "0.5",
                        "created": "2016-08-29 14:54:56",
                        "modified": "2016-09-27 17:51:57",
                        "ware_id": 10125192581,
                        "spu_id": 0,
                        "cid": 9730,
                        "vender_id": 209379,
                        "shop_id": 200753,
                        "ware_status": "ON_SALE",
                        "item_num": "DC3JA002S",
                        "upc_code": "",
                        "transport_id": 0,
                        "online_time": "2016-09-09 14:57:36",
                        "offline_time": "2016-08-29 14:54:55",
                        "cost_price": "0.00",
                        "market_price": "369.00",
                        "jd_price": "369.00",
                        "stock_num": 30
                  }
                  ],
            "m": ""
            }

```

12.SearchSkuList()            Sku搜索服务

 路由:`/core/Api/JdSku/SearchSkuList`
 
 参数：
```sh
      input:
            skuStatuValue		      SKU状态：1:上架 2:下架 4:删除  ,多选用英文逗号隔开
            startCreatedTime	            创建时间											
            endCreatedTime		      结束时间											
            pageNo				页码(此方法不能设置pageSize，默认20)												
      output:
            {
            "s": 1,
            "d": {
            "data": [
                  {
                  "jdPrice": 138,
                  "outerId": "N3L6F50021056170",
                  "skuId": 10225146725,
                  "status": 2,
                  "wareId": 10045763110
                  },
                  {
                  "jdPrice": 138,
                  "outerId": "N3L6F50021056175",
                  "skuId": 10225146726,
                  "status": 2,
                  "wareId": 10045763110
                  }],
            "pageNo": 1,
            "pageSize": 20,
            "totalItem": 3167
            },
            "m": ""
            }
```

13.DpsOutbound()            厂商直送出库

 路由:`/core/Api/JdSupplier/DpsOutbound`
 
 参数：
```sh
      input:
            token           授权码
```

14.DpsDelivery()            厂商直送发货

 路由:`/core/Api/JdSupplier/DpsDelivery`
 
 参数：
```sh
      input:
            token           授权码
```

15.EptDeliveryOrder()        订单发货

 路由:`/core/Api/JdSupplier/EptDeliveryOrder`
 
 参数：
```sh
      input:
            token           授权码
```

16.WayBillCodeget()        获取快递单号

 路由:`/core/Api/JdWayBill/WayBillCodeget`
 
 参数：
```sh
      input:
            token           授权码
            preNum          获取运单号数量（最大100）
            customerCode    商家编码（区分英文大小写） 此商家编码需由商家向京东快递运营人员（与商家签订京东快递合同的人）索取。 雅鹿男装旗舰店商家编码为 021K5449 （区分英文大小写）
            orderType       可为空，运单类型。(普通外单：0，O2O外单：1)默认为0
      output:
            {
                  "s": 1,
                  "d": [
                  "VB31961092656",
                  "VB31961471135"
                  ],
                  "m": ""
            }

```

17.EptDeliveryOrder()        是否可以京配

 路由:`/core/Api/JdWayBill/RangeCheck`
 
 参数：
```sh
      input:
            token           授权码
```

18.WaybillSend()        京东快递提交运单信息接口（青龙接单接口）

 路由:`/core/Api/JdWayBill/WaybillSend`
 
 参数：
```sh
      input:
            token           授权码
```

19.OrderPrint()        快递单打印

 路由:`/core/Api/JdWayBill/OrderPrint`
 
 参数：
```sh
      input:
            token           授权码
```

20.TraceGet()        快递单打印

 路由:`/core/Api/JdWayBill/TraceGet`
 
 参数：
```sh
      input:
            waybillCode     订单号
            token           授权码
      output:
            {
                  "s": 1,
                  "d": [
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "货物已交付京东快递",
                              "ope_time": "2016/09/15 08:03:04",
                              "ope_title": "快递签收"
                        },
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "货物已完成分拣，离开【苏州接货仓】",
                              "ope_time": "2016/09/15 08:03:34",
                              "ope_title": "分拣中心发货"
                        },
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "货物已完成分拣，离开【苏州接货仓】",
                              "ope_time": "2016/09/15 08:03:34",
                              "ope_title": "分拣中心发货"
                        },
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "货物已到达【苏州陆家分拣中心】",
                              "ope_time": "2016/09/15 09:33:01",
                              "ope_title": "分拣中心验货"
                        },
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "货物已完成分拣，离开【苏州陆家分拣中心】",
                              "ope_time": "2016/09/15 09:44:00",
                              "ope_title": "分拣中心发货"
                        },
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "货物已到达【常熟海虞站】",
                              "ope_time": "2016/09/15 15:17:24",
                              "ope_title": "站点收货"
                        },
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "货物已分配，等待配送",
                              "ope_time": "2016/09/15 15:17:25",
                              "ope_title": "站点验货"
                        },
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "配送员开始配送，请您准备收货，配送员，许绍锋，手机号，15851514418或18502586285",
                              "ope_time": "2016/09/15 15:35:11",
                              "ope_title": "配送员收货"
                        },
                        {
                              "ope_name": "京东快递",
                              "ope_remark": "货物已完成配送，感谢您选择京东配送",
                              "ope_time": "2016/09/15 15:37:43",
                              "ope_title": "妥投"
                        }
                        ],
                  "m": ""
            }
```

21.jdPackageUpdate()        修改京东快递包裹数

 路由:`/core/Api/JdWayBill/jdPackageUpdate`
 
 参数：
```sh
      input:
            customerCode    商家编号
            deliveryId      运单号
            packageCount    包裹数(大于0，小于1000)
            token           授权码
```

22.jdOrderIntercept()        运单拦截

 路由:`/core/Api/JdWayBill/OrderIntercept`
 
 参数：
```sh
      input:
            token           授权码
```






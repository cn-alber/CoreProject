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
                 [
                    {
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


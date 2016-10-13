接口文件：CoreWebApi/Controllers/Api/JingDong/JdOrderControllers.cs

function说明
==============
1.orderDownload()            下载订单

 路由:`/core/Api/JdOrder/download`
 
 参数：
```sh
    input: 
            {
                token           授权码
                fields          需要返回的字段列表，多个字段用半角逗号分隔，可选值为返回示例中能看到的所有字段
                start_created   查询三个月内交易创建时间开始。格式:yyyy-MM-dd HH:mm:ss
                end_created     查询交易创建时间结束。格式:yyyy-MM-dd HH:mm:ss
                status          交易状态
                buyer_nick      买家昵称
                type            交易类型列表
                ext_type        扩展类型
                rate_status     评价状态
                tag             卖家对交易的自定义分组标签
                page            页码
                pageSize        每页的条数（最大page_size 100条）
                use_has_next    默认 false
            }
    output:
            s: 1 || error_code
            d:[{
                "adjust_fee": "0.00",
                "buyer_nick": "花有重开时人无长少年",
                "buyer_obtain_point_fee": 0,
                "buyer_rate": false,
                "cod_fee": "0.00",
                "cod_status": "NEW_CREATED",
                "created": "2016-10-13 14:46:35",
                "discount_fee": "0.00",
                "modified": "2016-10-13 14:50:42",
                "num": 1,
                "num_iid": 523042653121,
                "orders": {
                    "order": [
                            {
                            "adjust_fee": "0.00",
                            "buyer_rate": false,
                            "discount_fee": "868.00",
                            "num": 1,
                            "num_iid": 523042653121,
                            "oid": 2429042494356176,
                            "outer_iid": "N3L4F58351",
                            "outer_sku_id": "N3L4F58351001175",
                            "payment": "320.00",
                            "pic_path": "http://img02.taobaocdn.com/bao/uploaded/i2/2058964557/TB2IN76gXXXXXX1XXXXXXXXXXXX_!!2058964557.jpg",
                            "price": "1188.00",
                            "refund_status": "NO_REFUND",
                            "seller_rate": false,
                            "seller_type": "B",
                            "sku_id": "3125450559380",
                            "sku_properties_name": "颜色:夜宴黑;尺码:175",
                            "status": "WAIT_SELLER_SEND_GOODS",
                            "title": "南极人冬季新款男士加厚羽绒服中年男装短款连帽外套休闲爸爸装潮",
                            "total_fee": "320.00"
                            }
                        ]
                    },
                "pay_time": "2016-10-13 14:49:11",
                "payment": "320.00",
                "pic_path": "http://img02.taobaocdn.com/bao/uploaded/i2/2058964557/TB2IN76gXXXXXX1XXXXXXXXXXXX_!!2058964557.jpg",
                "point_fee": 0,
                "post_fee": "0.00",
                "price": "1188.00",
                "real_point_fee": 0,
                "received_payment": "0.00",
                "receiver_address": "小**街道上海市黄浦区陆**路1**号",
                "receiver_city": "上海市",
                "receiver_district": "黄浦区",
                "receiver_mobile": "135********",
                "receiver_name": "李**",
                "receiver_state": "上海",
                "receiver_zip": "200010",
                "seller_nick": "南极人羽绒旗舰店",
                "seller_rate": false,
                "shipping_type": "express",
                "sid": "2429042494356176",
                "status": "WAIT_SELLER_SEND_GOODS",
                "tid": 2429042494356176,
                "title": "南极人羽绒旗舰店",
                "total_fee": "1188.00",
                "type": "fixed"
                },
            ]
            m: 
    
```
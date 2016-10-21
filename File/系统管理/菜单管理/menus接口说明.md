物理路径: CoreWebApi/Controllers/Base/AdminControllers.cs
接口名称：AdminControllers

function说明
===========================

1.获取菜单
    方法：Get

    路由：`/core/admin/menus`

    参数: CoreModels.XyUser.UserParam
    
    InPut:

    OutPut:    
```sh
            {
                "s": 1,
                "d": [
                    {
                        "id": 1,
                        "name": "系统管理",          //菜单名
                        "router": "",               //路径
                        "access": null,             //权限名称
                        "order": "1",               //排序索引
                        "remark": "顶级菜单",        //备注
                        "parentid": 0,              //父类
                        "icon": [
                            "gear",                 //图标
                            "fa"                    //图标前缀
                            ],
                        "children": []              //子菜单
                        },
                    ],
                "m": ""
            }
```

2.创建菜单

    方法：POST

    路由：`/core/admin/createmenus`
    
    参数: CoreModels.XyUser.UserParam
    
    InPut:
```sh
        id 			
        name 		
        router 		
        access		
        order		
        remark		
        parentid 	
        NewIcon 	
        NewIconPre 	
        icon 		
        children 	
```
    OutPut:    
```sh
            {
                "s": 1, || error_code
                "d": null                   
                "m": ""
            }
```
 3.编辑菜单

    方法：POST
    
    路由：`/core/admin/modifymenus`
   
    参数: CoreModels.XyUser.UserParam
    
    InPut:
```sh
        id 			
        name 		
        router 		
        access		
        order		
        remark		
        parentid 	
        NewIcon 	
        NewIconPre 	
        icon 		
        children 	
```
    OutPut:    
```sh
            {
                "s": 1, || error_code
                "d": null                   
                "m": ""
            }
```       

 4.获取单条菜单
    
    方法：POST
    
    路由：`/core/admin/onemenu`
    
    参数: CoreModels.XyUser.UserParam
    
    InPut:
```sh
        id 				
```
    OutPut:    
```sh
            {
                "s": 1, || error_code
                "d": 
                    "id": 1,
                    "name": "系统管理",          //菜单名
                    "router": "",               //路径
                    "access": null,             //权限名称
                    "order": "1",               //排序索引
                    "remark": "顶级菜单",        //备注
                    "parentid": 0,              //父类
                    "icon": [
                        "gear",                 //图标
                        "fa"                    //图标前缀
                        ],                   
                "m": ""
            }
```   
4.删除菜单
    
    方法：POST
    
    路由：`/core/admin/onemenu`
    
    参数: CoreModels.XyUser.UserParam
    
    InPut:
```sh
        ids   菜单ID 数组
```
    OutPut:    
```sh
            {
                "s": 1, || error_code
                "d": nulll                                       
                "m": ""
            }
```   

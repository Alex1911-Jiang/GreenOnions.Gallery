# GreenOnions.Gallery

这个项目的前身是自己[QQ机器人](https://github.com/Alex1911-Jiang/GreenOnions)的图库项目, 后来恰好自己在学习微服务就随手拿过来重构成了现在这幅模样 ~~所以表面是个图库，实际上是微服务的练习项目~~

如果你只是想~~看色图~~用Api，请直接打开http://alex1911.top/使用(由于是用的是自家的电脑所以随时可能出现如花生壳流量用光,Windows强制更新导致的重启,断网断电之类的情况,最极端可能我直接删库跑路)
图片大部分是从[Quan大佬](https://github.com/Quan666)的[ELF图库](http://img.shab.fun:5000/)~~DDOS~~请求来的，我还没来得及分类，请申请ApiKey后将分级类型设置为9获取。

#### 如果你想部署这个图库，请注意以下几点：

1.本项目数据库采用SqlServer，很大程度上宣告了只能在Windows平台部署<br>
2.本项目依赖.Net 5，即使是Windows系统也要求至少Windows7Sp1以上才能使用<br>
3.由于我完全不会前端所以界面很丑<br>

#### 项目优势:

1.采用微服务架构，页面和Api是分开的项目，可以部署在不同的设备上，方便管理和提高性能<br>
2.作为使用方，有ApiKey申请与销毁页面，方便应对Key泄露的情况<br>
3.作为部署方，有添加/编辑图片页面，方便图片数据管理<br>
4.可以指定返回值数据类型是Json或Xml，有着更好的兼容性<br>

#### 技术参数：

1.使用[Consul](https://github.com/hashicorp/consul)实现的服务注册与发现，可动态增减集群实例<br>
2.使用[Ocelot](https://github.com/ThreeMammals/Ocelot)实现的分布式网关，负载均衡<br>
3.使用JWT的权限认证<br>
4.使用[Polly](https://github.com/App-vNext/Polly)实现的缓存、限流、熔断<br>

#### 部署方式：

（真有人要部署再写）

#### 短期计划(TODO):

1.找回密码/修改密码功能<br>
2.修改昵称功能(并没有什么用)<br>
3.限制ApiKey的每日访问次数并限制申请ApiKey的间隔时间<br>

###### 长期计划(GUDO):

1.使用[Blazor](https://github.com/dotnet/blazor)或[MAUI](https://github.com/dotnet/maui)重构前端页面(看我先学会哪个)<br>
2.把数据库换成[Elasticsearch](https://github.com/elastic/elasticsearch)

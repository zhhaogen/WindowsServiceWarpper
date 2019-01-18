## Windows service wrapper
把任意exe程序或脚本封装成windows系统服务,重复造的轮子，其他推荐使用[winsw
](https://github.com/kohsuke/winsw)
### 特点
* 轻量级,仅仅一个主要文件
* 简单配置,采用非xml的ini配置
### 编译
使用Visual Studio Community 2017 打开
### 使用
复制WindowsServiceWarpper.exe到任意位置,可重命名,在同目录下建立warpservice.ini 文件,配置参考Resources\warpservice.ini

例如创建Nginx服务，配置
```
ServiceName = Nginx Service
#运行程序
Exe = E:\nginx-1.14.2\nginx.exe
#停止参数
StopArgs = -s stop
```
创建服务
```
sc create nginxSvr binPath= "E:\nginx-1.14.2\WindowsServiceWarpper.exe"  DisplayName="Nginx Service"
```
删除服务
```
sc delete nginxSvr
```
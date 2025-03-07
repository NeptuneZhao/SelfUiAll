### 关于 SelfUI

名字是自己随便取的，上一个项目基于 `WinUI 3.0` 制作，又刚好想到 `myself`，所以取了个这样一个名字。


至于创建这个项目，起因是心血来潮想学习 `HITA` 为 ~~我们班~~我自己做一个桌面课程系统，但是在本部的统一身份认证这里卡了很久。

于是和一位志同道合之友([@RastyASWoz](https://github.com/RastyASWoz)，下称小友)商议，在他的帮助下完成了认证的逆向工程。


### 关于"逆向"本部 IDS 流程

不像 `HITSZ-SSO`（感谢小友提供的事实），`HIT-IDS` 使用了基于 `AES-128` 的加密系统和 **单次登录验证码需求**（我自己起的名）的二重保障。

下面展示从IDS登录到目标门户的流程。

#### 第 0 步: 访问 IDS

统一身份认证需求的访问格式是：`https://ids.hit.edu.cn/authserver/login?service=(...)`

- 如果没有要登录的门户（即 `login` 后没有字段），系统会自动跳转到统一身份认证管理页面，具体过程略。最终页面是 `https://ids.hit.edu.cn/personalInfo/personCenter/index.html#/accountsecurity`。
- 如果登录的门户不在服务范围内（即不是一校三区内认证门户），系统也会成功返回 `200` 但是不会跳转，而是显示错误信息。

该项目从本部 `IDS` 登录到深圳本研系统，即 `https://ids.hit.edu.cn/authserver/login?service=http%3A%2F%2Fjw.hitsz.edu.cn%2FcasLogin`。该连接是安全的。

你会发现要重定向的地址包含 `CAS` 认证，这是由于深圳的 `SSO` 方式与本部不同，因此多加一步认证。

- 深圳的 `SSO` 认证是基于 `CAS` 的，我们从此看：`https://sso.hitsz.edu.cn:7002/cas/login?service=https%3a%2f%2fhitsz.mycospxk.com`。
- 我们发现深圳的门户系统是基于 `CAS` 和 `ticket` 的。


#### 第 1 步：获取"公钥"

在每次进入 `IDS` 时，浏览器（HttpClient）会GET本站，在该网站上有且仅有一个表单，该表单下存在一个隐藏输入框，内容为128位（16字节）的盐（密钥）。我们在这时暴力正则匹配即可。经实战分析，获取类下的属性远比匹配耗时（约600ms vs 约50ms）。
第2步：询问验证码
当用户从username框中移出时，浏览器会GET到需求验证码的服务，具体实现原理未知。目标服务器收到了两条信息，一个是输入的账户名，另一个是时间戳。返回值为JSON。如果此时需求图片验证码，则自动登录无法继续（有待验证）。
第三步：POST表单
如果上述目标达成，POST返回状态应该是302。以本部账户登录深圳本研系统为例。此时的302是从IDS到本研系统的CAS。
获取到身份信息后，CAS会返回302，给这时的浏览器一个登录凭据（明文ticket），我们带着返回的饼干和凭据从响应请求给出的Host和Location，就能到达本研系统的主页了！

### TODO

验证码使用 `OpenCVSharp` 获取坐标 [CSDN](https://blog.csdn.net/lzz718719/article/details/143592579)
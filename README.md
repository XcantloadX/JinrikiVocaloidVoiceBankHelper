# UTAU 人力音源制作助手
建议有大量素材时使用，比如超多文件、超长视频等等  

## 功能
* *半自动化语音识别（语音转文字） \***（由剪映/阿里云 API 提供）***
* *去伴奏、降噪 \***（由 Ultimate Vocal Remover 提供）***
* 素材库
* 自动载入素材库中的字幕文件并转换为拼音，**你可以在大量素材中直接搜索想要的字**
* 空格键试听搜索结果，**回车键用 Adobe Audition 打开素材，并自动定位到该句语音，便于编辑**
* **一键转换 Adobe Audition 当前选区为 UTAU wav 格式并自动编号保存到音源目录**
* (开发中) **自动将整句语音切分为单字语音(仅供参考，需要进一步手动微调，不能直接作为成品)**
* (开发中) **自动唤起 setParam 进行 oto 设定**
* (开发中) 标签功能，可以对音质差、已经使用过的音源、不同的说话人等等进行标记
* (计划中) 自动识别多个说话人

\* 依赖于其他软件，需要你手动安装

## 编译
使用 Visual Studio 2015 打开本项目，直接编译即可。  
需要注意的是，你需要手动下载 VvTalk 并解压到`tools\`目录下  
即需要出现这个目录 `tools\VvTalk`  
下载地址：https://www.vsqx.top/project/vn5823   

## 安装
TODO

## 教程
**注意：本工具部分功能（与 UTAU 自动化有关的功能，如自动刷新音源、自动打开原音设定等）依赖于[此汉化版的 UTAU](https://www.bilibili.com/read/cv7974422)，其他版本可能无法使用。**  
[简明使用教程](https://github.com/XcantloadX/JinrikiVocaloidVoiceBankHelper/wiki/%E7%AE%80%E6%98%8E%E6%95%99%E7%A8%8B)

## 感谢
本项目参考/引用了一下项目/软件：  
* [VvTalk](https://github.com/GalaxieT/VvTalk)
* setParam
* ffmpeg&ffplay
* [Adobe Extension Samples](https://github.com/Adobe-CEP/Samples)
* [Pinyin4NET](https://github.com/hyjiacan/Pinyin4NET)
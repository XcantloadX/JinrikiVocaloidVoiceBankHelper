/*//打开要处理的音频文件
var doc = app.openDocument(new DocumentOpenParameter("D:\\CloudMusic\\warma - 她追逐着月光的尽头.mp3"));
while(doc.isBusy){}

var startPos = 2.656;
var endPos = 3.765;
//移动播放头
//playheadPosition 单位是音频采样，需要转换一下
doc.playheadPosition = startPos * doc.sampleRate;
//设置入点为当前位置
app.invokeCommand(Application.COMMAND_EDIT_SETINPOINTTOCTI);
//同理
doc.playheadPosition = endPos * doc.sampleRate;
app.invokeCommand(Application.COMMAND_EDIT_SETOUTPOINTTOCTI);

//导出文件
app.invokeCommand(Application.COMMAND_EDIT_COPYTONEW); //复制选区到新建
while(app.activeDocument.path != ""){} //等待新建操作完成
// $.sleep (300) //不然新建的音频文件还没加载好
var newDoc = app.activeDocument;
var ret = newDoc.applyFavorite("UTAU"); //改变音频格式
while(newDoc.isBusy){}; //等待操作完成
if(!ret)
    $.writeln("Error: Failed to convert to UATU .wav format");
newDoc.saveAs("D:\\test.wav"); 
while(newDoc.isBusy){}; //等待操作完成
newDoc.closeDocument();
*/


function openFile(path){
    var doc = app.openDocument(new DocumentOpenParameter(path));
    while(doc.isBusy){}
}

function select(start, end){
    var doc = app.activeDocument;
    app.invokeCommand(Application.COMMAND_TRANSPORT_STOP); //停止当前播放
    //同理
    doc.playheadPosition = end * doc.sampleRate;
    app.invokeCommand(Application.COMMAND_EDIT_SETOUTPOINTTOCTI);
    //设置入点为当前位置
    doc.playheadPosition = start * doc.sampleRate;
    app.invokeCommand(Application.COMMAND_EDIT_SETINPOINTTOCTI);
    app.invokeCommand(Application.COMMAND_VIEW_ZOOMTOSELECTION); //缩放选区
}

function seek(time){
    var doc = app.activeDocument;
    app.invokeCommand(Application.COMMAND_TRANSPORT_STOP); //停止当前播放
    doc.playheadPosition = time * doc.sampleRate;
}

function saveSelection(path){
    //导出文件
    app.invokeCommand(Application.COMMAND_EDIT_COPYTONEW); //复制选区到新建
    while(app.activeDocument.path != ""){} //等待新建操作完成
    // $.sleep (300) //不然新建的音频文件还没加载好
    var newDoc = app.activeDocument;
    var ret = newDoc.applyFavorite("UTAU"); //改变音频格式
    while(newDoc.isBusy){}; //等待操作完成
    if(!ret)
        $.writeln("Error: Failed to convert to UATU .wav format");
    newDoc.saveAs(path); 
    while(newDoc.isBusy){}; //等待操作完成
    newDoc.closeDocument();
}

console.log("debug");
let csInterface = new CSInterface();
let statusEle = document.getElementById("status");
let msgEle = document.getElementById("msg");

window.setInterval(function(){
    fetch("http://localhost:2233/getNextCommand")
        .then((response) => response.json())
        .then((obj) => {
            if(obj.message != "ok"){
                error();
                statusEle.innerHTML = "Error: " + obj.message;
                return;
            }
            
            statusEle.innerHTML = "Connected";
            ok();
            if(obj.command != "" && obj.command != null){
                
                console.log(obj.command);
                //csInterface.evalScript(obj.command);
                eval(obj.command);
            }
            msgEle.innerHTML = obj.message;
        })
        .catch((e) => {
            error()
            statusEle.innerHTML = "Error/Disconnnected";
            msgEle.innerHTML = e.toString();
            console.log(e);
        });
}, 50);

function error(){
    statusEle.className = "error";
    msgEle.className = "error";
}

function ok(){
    statusEle.className = "success";
    msgEle.className = "success";
}
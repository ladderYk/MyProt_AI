
export const getProtocolList=(form)=>{
 if (chrome.webview) {
    const Protocol = chrome.webview.hostObjects.Protocol;
    return Protocol.getProtocolList(JSON.stringify(form)).then(handleRet);
  }
  return Promise.reject("错误");
}

export const delProtocol=(name)=>{
 if (chrome.webview) {
    const Protocol = chrome.webview.hostObjects.Protocol;
    Protocol.delProtocol(name).then();
  }
  return Promise.reject("错误");
}
export const addProtocol=(form)=>{
 if (chrome.webview) {
    const Protocol = chrome.webview.hostObjects.Protocol;
    Protocol.addProtocol(JSON.stringify(form)).then();
  }
  return Promise.reject("错误");
}
export const editProtocol=(form)=>{
 if (chrome.webview) {
    const Protocol = chrome.webview.hostObjects.Protocol;
    Protocol.editProtocol(JSON.stringify(form)).then();
  }
  return Promise.reject("错误");
}
const handleRet = (strResponse) => {
  var response = JSON.parse(strResponse);
  if (!response) return;
  return response["data"] ? response.data : response.result;
};

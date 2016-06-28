$(document).ready(function () {
    //alert($.browser.msie + " - " + $.browser.version);
    if (sessionStorage.user_code != null) {
        window.location.href=(client.main.config.baseURL+"/WebClient/");
    }; 
});
function checkLetters(str) {
    var check = false;
    if ((str.indexOf("<") > -1) || (str.indexOf(">") > -1))check = true;
    return check;
}

function show(num) {
	var name1 = "customRange" + num;
	var name2 = "price" + num;
	var a = document.getElementById(name1).value;
	document.getElementById(name2).value = Number(a).toLocaleString();
}

function checkchange(num) {
	var name = "checkbox" + num;
	var item = document.getElementById(name);
	if (item.value == "true")
		item.value = "false"; else
		item.value = "true";
}



function changesort(num) {
	var name = "hiddensort";
	document.getElementById(name).value = num;	
}

function changenum(num) {
	var name = "hiddennum";
	document.getElementById(name).value = num;

}


function downpage() {
	var num = Number(document.getElementById("pagenum").value);
	if (num > 1) {
		num--;
		document.getElementById("pagenum").value = num;
		document.getElementById("btnpage1").value = num;
	}

}


function uppage() {
	var num = Number(document.getElementById("pagenum").value);
	num++;
	document.getElementById("pagenum").value = num;
	document.getElementById("btnpage1").value = num;

}

function drop() {
	document.getElementById("act").value = "drop";
}

function search() {
	document.getElementById("act").value = "search";
}

function changemail() {
	var name = document.getElementById("mail").value;
	document.getElementById("mailname").value = name;

	var tg = document.getElementById("telegram").value;
	document.getElementById("tg").value = tg;

}
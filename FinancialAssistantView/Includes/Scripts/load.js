getMarkets();
document.getElementById('markets').onchange = function () {
	getBoards();
	/*
	let text2 = document.querySelector('.text2');
	let e2 = document.getElementById('boards');
	text2.innerText = e2.options[e2.selectedIndex = 0].text;
	e2.classList.add('hide');

	let text = document.querySelector('.text');
	let e = document.getElementById('markets');
	text.innerText = e.options[e.selectedIndex].text;
	e.classList.add('hide');
	*/
}
/*
document.getElementById('boards').onchange = function () {
	let text2 = document.querySelector('.text2');
	let e2 = document.getElementById('boards');
	text2.innerText = e2.options[e2.selectedIndex].text;
	e2.classList.add('hide');
}
*/
document.getElementById('find').onclick = function () {
	getTools();
}
async function getMarkets(){
	let responce = await fetch('https://localhost:7175/FinancialAssistant/Markets');
	let content =  await responce.text();
	let jsoncontent = JSON.parse(content);
	var markets = jsoncontent["markets"]["data"];

	for(let i = 0; i < markets.length; i++){
		var option = document.createElement('option');
		option.name = markets[i][1];
		option.value = markets[i][1];
		option.className = 'market';
		option.innerText = markets[i][2];
		document.querySelector('.markets').appendChild(option);
	}
}
async function getBoards(){
	let market = document.getElementById('markets').value;
    let url = 'https://localhost:7175/FinancialAssistant/Boards?market=' + market;

	let responce = await fetch(url);
	let content =  await responce.text();
	let jsoncontent = JSON.parse(content);
    var boards = jsoncontent["boards"]["data"];
	var element = document.querySelector(".boards");

	while (element.firstChild){
  		element.removeChild(element.firstChild);
	}

    for (let i = 0; i < boards.length; i++){
		var option = document.createElement('option');
		option.name = boards[i][2];
		option.value = boards[i][2];
		option.innerText = boards[i][3];
		option.className = 'board';
		document.querySelector('.boards').appendChild(option);
    }
}
async function getTools(){
	let advice = document.querySelector('.advice');
	advice.innerText = 'Поиск нужных инструментов...';

	let params = (new URL(document.location)).searchParams;
    let market = document.getElementById('markets').value;
    let board = document.getElementById('boards').value;
    let url = 'https://localhost:7175/FinancialAssistant/Get?market=' + market + "&board=" + board;

	let responce = await fetch(url);
	let content =  await responce.text();
	let jsoncontent = JSON.parse(content);
	
	var element = document.querySelector('.toolsul');

	while (element.firstChild){
		element.removeChild(element.firstChild);
  	}

    for (let i = 0; i < jsoncontent.length; i++){
        var li = document.createElement('li');
		var span = document.createElement('span');
		span.className = 'toolboard';
		li.innerHTML = `<span class="toolboard">${(jsoncontent[i]['BoardName']).substring(0,4)}</span>${jsoncontent[i]['ToolName']}`;
		if (i % 2 != 0){
			li.innerHTML = `<span class="toolboard" style="background-color: #2B5DE0">${(jsoncontent[i]['BoardName']).substring(0,4)}</span>${jsoncontent[i]['ToolName']}`;
		}
		li.className = 'tool';
		li.id = 'tool';
		li.classList.add('tool');
		li.classList.add('chartStart');
		document.querySelector('.toolsul').appendChild(li);
    }

	advice.classList.add('hideAdvice');
}
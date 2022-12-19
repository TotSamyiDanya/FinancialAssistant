var tools = document.getElementById('tools');
tools.onclick = function (e) {
	let target = e.target;
  let board = document.getElementById('boards').value;
	let name = target.innerText.substring(4,target.innerHTML.length);
  getTool(name,board);
}
Chart.defaults.font.size = 15;
Chart.defaults.font.family = 'Montserrat';
Chart.defaults.color = '#2d2b36';
Chart.defaults.font.weight = 500;
Chart.defaults.layout.padding = 15;
let myChart = new Chart(
  document.getElementById('MyChart'),
);
function CreateChartShare(toolJson){
  let tradeDates = toolJson["TradeDates"];
  let prices = [];
  let dates = [];

  for(let i = 0; i < tradeDates.length; i++){
    prices[i] = tradeDates[i]["Price"];
    dates[i] = tradeDates[i]["Date"];
  }
  let means = meanSmooth(prices.reverse().slice(prices.length-60,prices.length));
  let result = '';
  console.log(means);
  if (prices[prices.length - 1] < means[means.length - 1] && prices[prices.length - 2] - prices[prices.length - 1] < 0){
    result =  'Купить';
  }
  else {
    result = 'Продать'
  }
  let data = {
    labels: dates.reverse().slice(dates.length-30, dates.length),
    datasets: [{
      label: 'Стоимость инструмента',
      backgroundColor: 'rgb(249,113,116)',
      borderColor: 'rgb(249,113,116)',
      data: prices.slice(prices.length-30,prices.length),
    },
    {
      label: 'Средняя скользящая',
      backgroundColor: 'rgb(92,114,240)',
      borderColor: 'rgb(92,114,240)',
      borderDash: [5,5],
      data: means.slice(means.length-30,means.length),
    }]
  };
  let options = {
    responsive: true,
    plugins: {
      title: {
      display: true,
      text: `    ${toolJson["BoardName"].substring(0,4)} ` + toolJson["ToolName"],
      font: {
        size: 31
      },
      align: 'start'
    },
    subtitle: {
      display: true,
      text: 'Рекомендуется: ' + `${result} `,
      font: {
        size: 31,
        weight: 700
      },
      align: 'end'
    },
    legend: {
      position: 'bottom'
    }
  }
};
  let config = {
    type: 'line',
    data: data,
    options: options
  };
  
  myChart.destroy();
  myChart = new Chart(
    document.getElementById('MyChart'),
    config
    );
}
function meanSmooth(prices){
  let result = [];
  for (let i = 30; i < prices.length; i++){
    let res = lastThirtySum(i,prices) / 30;
    result.push(res);
  }
  return result;
}
function lastThirtySum(p, array){
  let result = 0;
  for (let i = p - 30; i < p + 1; i++){
    result = result + array[i];
  }
  return result;
}
function checkResult(prices, means, length) {
  alert(prices[length]);
  if (prices[length] < means[length] && prices[length - 1] - prices[length] < 0){
    return 'Купить';
  }
  else {
    return 'Продать'
  }
}
async function getTool(toolName, toolBoard){
  let url = 'https://localhost:7175/FinancialAssistant/Tool?toolName=' + toolName + "&board=" + toolBoard;
  let responce = await fetch(url);
  let content =  await responce.text();
  let jsoncontent = JSON.parse(content);
  let market = jsoncontent["MarketName"];
  let chart = document.querySelector('.chart');
  chart.classList.add('chartStart');
  switch(market){
    case 'shares':
      CreateChartShare(jsoncontent);
      break;
  }
}
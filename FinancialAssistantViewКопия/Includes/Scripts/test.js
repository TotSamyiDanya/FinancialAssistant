const dates = [
  '02.11.22','01.11.22','31.10.22','28.10.22','27.10.22','26.10.22','25.10.22','24.10.22','21.10.22',
  '20.10.22','19.10.22','18.10.22','17.10.22','14.10.22','13.10.22','12.10.22','11.10.22','10.10.22',
  '07.10.22','06.10.22','05.10.22','04.10.22','03.10.22','02.10.22','01.10.22','31.09.22','28.09.22',
  '27.09.22','26.09.22','25.09.22','24.09.22'
];

const toolName = 'Абрау Дюрсо';

const datePrice = [
  179, 181, 181.5, 181, 180, 181.5, 180, 181, 179.5, 180, 180, 177, 178.5, 178, 172.5, 171, 170, 
  171, 168.5, 166.5, 169.5, 169.5, 170.5, 167, 162.5, 157.5, 165, 165.5, 158.5, 167, 178.5, 178
];

function meanSmooth(){
  const result = [];
  for (let i = 0; i < dates.length - 1; i++) {
    let res = (datePrice[i+1] + datePrice[i]) / 2;
    result.push(res);
  }
  return result;
}

let means = meanSmooth();


const data = {
  labels: dates,
  datasets: [{
    label: toolName,
    backgroundColor: 'rgb(249,113,116)',
    borderColor: 'rgb(249,113,116)',
    data: datePrice,
  },
  {
    label: 'Средняя скользящая',
    backgroundColor: 'rgb(92,114,240)',
    borderColor: 'rgb(92,114,240)',
    borderDash: [5,5],
    data: means,
  }]
};

 const config = {
    type: 'line',
    data: data,
    options: {
       title: {
            display: true,
            text: 'Пример Chart.js'
        },
        legends: {
          display: false
        }
    }
  };
const myChart = new Chart(
    document.getElementById('MyChart'),
    config
  );
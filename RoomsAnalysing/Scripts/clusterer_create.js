

ymaps.ready(init);

function init() {
    a = JSON.parse(document.getElementById("data").innerText);
    const myMap = new ymaps.Map("map", {
        center: [55.76, 37.64], // Москва
        zoom: 10

    });

    // определяем шаблон вывода суммы в иконке кластера
    const MyIconContentLayout = ymaps.templateLayoutFactory.createClass(
        "<span style='color: #000;'>{{ orderSum }}</span>"
    );

    // определяем ObjectManager
    const objectManager = new ymaps.ObjectManager({
        clusterize: true,
        clusterIconContentLayout: MyIconContentLayout
    });

    // Симуляция ajax запроса
    setTimeout(function () {
        // данные которые пришли с сервера
        data = JSON.parse( document.getElementById("data").innerText);

        // формируем коллекцию объектов карты
        const features = data.map(element => {
            return {
                type: "Feature",
                geometry: {
                    type: "Point",
                    coordinates: [element.lat, element.lon]
                },
                data: {
                    order: element.price
                }
            };
        });

        const collection = {
            type: "FeatureCollection",
            features
        };

        // добаляем коллекцию объектов в манагер
        objectManager.add(collection);

        // добавляем манагер на карту для вывода объектов
        myMap.geoObjects.add(objectManager);

        // проходимся по всем видимым кластерам, производим суммирование data.order во всех объектах и записываем полученное значение в options кластера
        objectManager.clusters.each(cluster => {
            let orderSum = 0;
            let num = 0;
            cluster.features.forEach(pin => {
                orderSum += parseFloat(pin.data.order);
                num++;
            });
            
            cluster.orderSum = Number(Math.round(orderSum / num)).toLocaleString();
            
        });

        // тоже самое что и выше только при изменении кластера
        objectManager.clusters.events.add("add", event => {
            let orderSum = 0;
            event.get("child").features.forEach(pin => {
                orderSum += parseFloat(pin.data.order);
            });

            objectManager.clusters.setClusterOptions(
                event.get("child").id, {
                orderSum
            }
            );
        });
    }, 1000);
}

  



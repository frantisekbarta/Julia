let canvas = document.getElementById('canvas');
let context = canvas.getContext('2d');
let imageData = context.createImageData(700, 700);

let barva = {
    r: 0,
    g: 0,
    b: 0
}

function Vygenerovat(c1, c2) {
    let t1 = performance.now();
    c1 = c1.replace(",", ".");
    c2 = c2.replace(",", ".");
    c1 = parseFloat(c1);
    c2 = parseFloat(c2);
    let pocetBarev = document.querySelector('input[name="VyberPoctuBarev"]:checked').id;
    let pozicePixelu;
    let pocetIteraci;
    let iterace = new Array(700 * 700);
    let cetnostiIteraci = new Array(256);

    for (let i = 0; i < cetnostiIteraci.length; i++) {
        cetnostiIteraci[i] = 0;
    }

    let pocetOdstinu = 1;

    for (let i = 0; i < 700; i++) {
        for (let j = 0; j < 700; j++) {
            pocetIteraci = VyhodnoceniBodu(2 * (i / 700 - 0.5), 2 * (j / 700 - 0.5), c1, c2, 256)
            if (pocetBarev == "JednaBarva")
                PrevodNa1Barvu(pocetIteraci);
            if (pocetBarev == "DveBarvy")
                PrevodNa2Barvy(pocetIteraci);
            pozicePixelu = (i + j * 700) * 4;
            imageData.data[pozicePixelu] = barva.r;
            imageData.data[pozicePixelu + 1] = barva.g;
            imageData.data[pozicePixelu + 2] = barva.b;
            imageData.data[pozicePixelu + 3] = 255;
            iterace[i * j] = pocetIteraci;
        }
    }

    context.putImageData(imageData, 0, 0);

    for (let i = 0; i < iterace.length; i++) {
        if (iterace[i] <= 255)
            cetnostiIteraci[iterace[i]]++;
    }

    for (let i = 0; i < cetnostiIteraci.length; i++) {
        if (cetnostiIteraci[i] > 0)
            pocetOdstinu++;
    }

    let t2 = performance.now();
    document.getElementById("Info").innerHTML = "počet odstínů: " + pocetOdstinu + ", čas: " + (t2 - t1).toFixed() + " ms";
}

function Nahodne() {
    c1 = ((Math.random() - 0.5) * 2).toFixed(3);
    c2 = ((Math.random() - 0.5) * 2).toFixed(3);
    c1 = c1.replace(".", ",");
    c2 = c2.replace(".", ",");
    document.getElementById("c1").value = c1;
    document.getElementById("c2").value = c2;
    Vygenerovat(c1, c2);
}

function Vyhledat() {
    let c1;
    let c2;
    let iterace = new Array(100 * 100);
    let cetnostiIteraci = new Array(256);

    for (let i = 0; i < cetnostiIteraci.length; i++) {
        cetnostiIteraci[i] = 0;

    }
    let pocetOdstinu;

    do {
        c1 = Math.round(((Math.random() - 0.5) * 2) * 1000) / 1000;
        c2 = Math.round(((Math.random() - 0.5) * 2) * 1000) / 1000;
        pocetOdstinu = 1;

        for (let i = 0; i < 100; i++) {
            for (let j = 0; j < 100; j++) {
                iterace[i * j] = VyhodnoceniBodu(2 * (i / 100 - 0.5), 2 * (j / 100 - 0.5), c1, c2, 256);
            }
        }

        for (let i = 0; i < iterace.length; i++) {
            if (iterace[i] <= 255)
                cetnostiIteraci[iterace[i]]++;
        }

        for (let i = 0; i < cetnostiIteraci.length; i++) {
            if (cetnostiIteraci[i] > 0)
                pocetOdstinu++;
        }

    } while (pocetOdstinu <= 75);

    c1 = c1.toString().replace(".", ",");
    c2 = c2.toString().replace(".", ",");
    document.getElementById("c1").value = c1;
    document.getElementById("c2").value = c2;
    Vygenerovat(c1, c2);
}

function VyhodnoceniBodu(polohaX, polohaY, c1, c2, maximalniPocetIteraci) {
    let pocetIteraci = 0;
    let z1 = polohaX;
    let z2 = polohaY;
    let z1Puvodni;
    let z2Puvodni;

    do {
        z1Puvodni = z1;
        z2Puvodni = z2;
        z1 = z1Puvodni * z1Puvodni - z2Puvodni * z2Puvodni + c1;
        z2 = 2 * z1Puvodni * z2Puvodni + c2;
        pocetIteraci++;
    }
    while ((z1 * z1 + z2 * z2) < 4 && pocetIteraci < maximalniPocetIteraci);

    return pocetIteraci;
}

function PrevodNa1Barvu(pocetIteraci) {
    let koeficient = 1 - pocetIteraci / 256;
    barva.r = koeficient * 255 + (1 - koeficient) * 0;
    barva.g = koeficient * 255 + (1 - koeficient) * 0;
    barva.b = koeficient * 255 + (1 - koeficient) * 0;
}

function PrevodNa2Barvy(pocetIteraci) {
    let koeficient = 1 - pocetIteraci / 256;
    if (pocetIteraci <= 128) {
        barva.r = koeficient * 255 + (1 - koeficient) * 0;
        barva.g = koeficient * 255 + (1 - koeficient) * 0;
        barva.b = koeficient * 255 + (1 - koeficient) * 255;
    }
    else if (pocetIteraci > 128 && pocetIteraci <= 256) {
        barva.r = koeficient * 0 + (1 - koeficient) * 255;
        barva.g = koeficient * 0 + (1 - koeficient) * 0;
        barva.b = koeficient * 255 + (1 - koeficient) * 0;
    }
    else {
        barva.r = 255;
        barva.g = 0;
        barva.b = 0;
    }
}

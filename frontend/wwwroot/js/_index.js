// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
//Comment
// Write your Javascript code.

if (document.body.contains(document.getElementById('calculator-Container'))) {
  init();
  selectFromDeck();
}

var playround = {
  boardcards: [],
  handcards: [],
  opponent: '1',
};

var boardcounter = 0;
var handcounter = 0;
var selectHoverColor = 'background-color:rgba(255, 255, 126, 0.64);';
var unselectColor = 'background-color:#001;';
var playroundInput = document.getElementById('playround');

function init() {
  var boardCards = document.getElementsByClassName('card-containier');
  Array.prototype.forEach.call(boardCards, function (card) {
    card.addEventListener('click', function () {
      select(card);
    });
    addMousehover(card, selectHoverColor, unselectColor);
  });
  var selector = document.getElementById('opponentSelector');
  selector.addEventListener('change', function () {
    playround.opponent = selector.value;
    playroundInput.value = JSON.stringify(playround);
    console.log(playround);
  });
}

function addMousehover(element, incolor, outcolor) {
  element.addEventListener('mouseover', function () {
    if (element.getAttribute('empty') == 'true') {
      element.setAttribute('style', incolor);
    }
  });
  element.addEventListener('mouseout', function () {
    if (
      element.getAttribute('selected') == 'false' &&
      element.getAttribute('empty') == 'true'
    ) {
      element.setAttribute('style', outcolor);
    }
  });
}

function setEmptyFieldColor() {
  var boardCards = document.querySelectorAll('[empty="true"]');
  Array.prototype.forEach.call(boardCards, function (card) {
    card.setAttribute('selected', 'false');
    card.setAttribute('style', unselectColor);
  });
  var newSelection = document.querySelector('[empty="true"]');
  newSelection.setAttribute('selected', 'true');
  newSelection.setAttribute('style', selectHoverColor);
}

function select(element) {
  if (element.getAttribute('empty') == 'true') {
    var boardCards = document.querySelectorAll('[empty="true"]');
    Array.prototype.forEach.call(boardCards, function (card) {
      card.setAttribute('selected', 'false');
      card.setAttribute('style', unselectColor);
    });
    if (element.getAttribute('selected') == 'false') {
      element.setAttribute('selected', 'true');
      element.setAttribute('style', selectHoverColor);
    }
  } else {
    element.setAttribute('style', selectHoverColor);
    element.setAttribute('empty', 'true');
    element.setAttribute('selected', 'true');
    removeCard(element);
    setEmptyFieldColor();
  }
}

function selectFromDeck() {
  var deckCards = document.getElementsByClassName('deck-card');
  //var selectedField = document.querySelector('[selected="true"]');
  Array.prototype.forEach.call(deckCards, function (card) {
    card.addEventListener('click', function () {
      //select(card)
      if (
        document.querySelectorAll('[empty="true"]').length != 0 &&
        card.getAttribute('selected-deckcard') == 'false'
      ) {
        card.setAttribute('selected-deckcard', 'true');
        card.setAttribute('style', 'background-color:gray;');
        setSelectedCard(card);
      }
    });
  });
}

function setSelectedCard(element) {
  var selectedField = document.querySelector('[selected="true"]');
  // selectedField.setAttribute("card", element.getAttribute("class"));
  selectedField.setAttribute('style', 'background-color:white;');
  selectedField.setAttribute('empty', 'false');
  selectedField.setAttribute('selected', 'false');
  addCard(selectedField, element);

  if (document.querySelectorAll('[empty="true"]').length != 0) {
    var newSelection = document.querySelector('[empty="true"]');
    newSelection.setAttribute('selected', 'true');
    newSelection.setAttribute('style', selectHoverColor);
  }
}

function addCard(field, deckCard) {
  var iconClassValue =
    'bi bi-suit-' + deckCard.getAttribute('cardType') + '-fill';
  var usedCard = document.createElement('div');
  //usedCard.setAttribute("style", "border-radius: 5px;");
  usedCard.setAttribute('cardType', deckCard.getAttribute('cardType'));
  usedCard.setAttribute('cardValue', deckCard.getAttribute('cardValue'));
  usedCard.setAttribute('class', 'used-card');
  var icon = document.createElement('i');
  icon.setAttribute('class', iconClassValue);
  var cardValContainer = document.createElement('div');
  cardValContainer.appendChild(
    document.createTextNode(deckCard.getAttribute('cardValue'))
  );
  usedCard.appendChild(icon);
  usedCard.appendChild(cardValContainer);
  usedCard.addEventListener('mouseover', function () {
    usedCard.setAttribute('style', 'background-color:rgba(122,122,122,0.6);');
  });
  usedCard.addEventListener('mouseout', function () {
    usedCard.setAttribute('style', 'background-color:white;');
  });
  field.appendChild(usedCard);
  if (field.getAttribute('handcard') == 'true') {
    handcounter++;
    disableEnableSubmitButton();
    var card = {
      cardType: deckCard.getAttribute('cardType'),
      cardValue: deckCard.getAttribute('cardValue'),
    };
    playround.handcards.push(card);
    //console.log(handcards);
    console.log(playround);
  } else {
    boardcounter++;
    disableEnableSubmitButton();
    var card = {
      cardType: deckCard.getAttribute('cardType'),
      cardValue: deckCard.getAttribute('cardValue'),
    };
    playround.boardcards.push(card);
    //console.log(boardcards);
    console.log(playround);
  }
  playroundInput.value = JSON.stringify(playround);
}

function disableEnableSubmitButton() {
  var calcButton = document.getElementById('calculateValueButton');
  if (handcounter >= 2 && boardcounter >= 3) {
    calcButton.disabled = false;
    console.log(handcounter);
    console.log(boardcounter);
  } else {
    calcButton.disabled = true;
    console.log(handcounter);
    console.log(boardcounter);
  }
}

function removeCard(element) {
  var usedCard = element.firstElementChild;
  //console.log(usedCard.getAttribute("cardType"))
  var deckCard = document.querySelector(
    '[id="' +
      usedCard.getAttribute('cardType') +
      usedCard.getAttribute('cardValue') +
      '"]'
  );
  deckCard.setAttribute('selected-deckcard', 'false');
  deckCard.setAttribute('style', 'background-color:white;');

  var card = {
    cardType: usedCard.getAttribute('cardType'),
    cardValue: usedCard.getAttribute('cardValue'),
  };
  if (element.getAttribute('handcard') == 'true') {
    handcounter--;
    disableEnableSubmitButton();
    playround.handcards = removeCardfromCards(playround.handcards, card);
    //console.log(handcards);
    console.log(playround);
  } else {
    boardcounter--;
    disableEnableSubmitButton();
    playround.boardcards = removeCardfromCards(playround.boardcards, card);
    console.log(playround);
  }
  playroundInput.value = JSON.stringify(playround);
  while (element.firstChild) {
    element.removeChild(element.firstChild);
  }
}

function removeCardfromCards(cards, card) {
  return cards.filter(
    (el) =>
      el.cardType != card.cardType ||
      el.cardValue != card.cardValue ||
      (el.cardType != card.cardType && el.cardValue != card.cardValue)
  );
}

//function addValuesToHiddenInputs() {
//    var valButton = document.getElementById("calculateValueButton");
//    valButton.addEventListener("click", function () {
//        var boardcards = document.querySelectorAll('.board-card');
//        var handcards = document.querySelectorAll('.player-card');
//        boardcards.forEach(function (card) {
//            //retrieve Every card on board
//        });
//        handcards.forEach(card => console.log(card.getAttribute("class")));
//    });
//}
//addValuesToHiddenInputs();

$(document).ready(function () {
  console.log('TAB');
  addTabFunc();
});

const activeStyle =
  'background-color:#ccc; pointer-events: none; text-decoration:none; cursor:not-allowed;';
const inactiveStyle = 'background-color:#f1f1f1; opacity: 0.5;';

const addTabFunc = () => {
  var tablinks = $('.gameTypeTab');
  setInitialAcitiveTab(tablinks);
  Array.prototype.forEach.call(tablinks, function (tab) {
    tab.addEventListener('click', (evt) => {
      tablinks.each((_, tablink) => {
        setInactive(tablink);
      });
      setActive(evt.currentTarget);
    });
  });
};

const setInitialAcitiveTab = (tablinks) => {
  for (var i = 1; i < tablinks.length; i++) {
    setInactive(tablinks[i]);
  }
  setActive(tablinks[0]);
};

const setInactive = (tab) => {
  tab.className = tab.className.replace(' active', '');
  tab.setAttribute('style', inactiveStyle);
};

const setActive = (tab) => {
  tab.className += ' active';
  tab.setAttribute('style', activeStyle);
};

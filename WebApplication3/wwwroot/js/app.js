let openShopping = document.querySelector('#openShopping');
let trialbody = document.querySelector('.trialbody');
let closeShopping = document.querySelector('#closeShopping');

openShopping.addEventListener('click', () => {
  
    trialbody.classList.add('active');
})

closeShopping.addEventListener('click', () => {
    trialbody.classList.remove('active');
})

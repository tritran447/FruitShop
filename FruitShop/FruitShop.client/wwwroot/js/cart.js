const CART_KEY = 'myShopCart';

function getCart() {
    const json = sessionStorage.getItem(CART_KEY);
    return json ? JSON.parse(json) : [];
}

function saveCart(cart) {
    sessionStorage.setItem(CART_KEY, JSON.stringify(cart));
}

function getCartDistinctCount() {
    return getCart().length;
}

function refreshCartCount() {
    const el = document.getElementById('cartCount');
    if (el) el.textContent = getCartDistinctCount();
}

function updatePlaceOrderBtn() {
    const btn = document.getElementById('placeOrderBtn');
    if (!btn) return;
    btn.disabled = (getCart().length === 0);
}

function addToCart(pid, name, price, img, quantity = 1) {
    const cart = getCart();
    const idx = cart.findIndex(it => it.productId === pid);
    if (idx >= 0) {
        cart[idx].quantity += quantity;
    } else {
        cart.push({ productId: pid, productName: name, price, imageUrl: img, quantity });
    }
    saveCart(cart);
    refreshCartCount();
}


function removeFromCart(pid) {
    const cart = getCart().filter(it => it.productId !== pid);
    saveCart(cart);
    refreshCartCount();
    renderCartTable();
}

function renderCartTable() {
    const cart = getCart();
    const tbody = document.getElementById('cartBody');
    if (!tbody) return;

    tbody.innerHTML = '';
    if (cart.length === 0) {
        tbody.innerHTML = `
          <tr>
            <td colspan="6" class="text-center py-4">Your cart is empty.</td>
          </tr>`;
        document.getElementById('totalAmount').textContent = '$0.00';
    } else {
        cart.forEach(it => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
              <td><img src="/assets/images/product-fruit/${it.imageUrl}" height="60" alt="${it.productName}" /></td>
              <td>${it.productName}</td>
              <td>$${it.price.toFixed(2)}</td>
              <td>${it.quantity}</td>
              <td>$${(it.price * it.quantity).toFixed(2)}</td>
              <td>
                <button class="btn btn-sm btn-danger remove-btn" data-pid="${it.productId}">
                  Remove
                </button>
              </td>`;
            tbody.append(tr);
        });
        const total = cart.reduce((s, it) => s + it.price * it.quantity, 0);
        document.getElementById('totalAmount').textContent = `$${total.toFixed(2)}`;
    }

    updatePlaceOrderBtn();
}

function showAddToast() {
    const el = document.createElement('div');
    el.className = 'toast align-items-center text-white bg-success border-0 position-fixed';
    el.style.cssText = 'top:1rem; right:1rem; z-index:1050;';
    el.setAttribute('role', 'alert');
    el.setAttribute('aria-live', 'assertive');
    el.setAttribute('aria-atomic', 'true');
    el.innerHTML = `
    <div class="d-flex">
      <div class="toast-body">Added to cart!</div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto"
              data-bs-dismiss="toast" aria-label="Close"></button>
    </div>`;
    document.body.append(el);
    new bootstrap.Toast(el, { delay: 1500 }).show();
}

document.addEventListener('DOMContentLoaded', () => {
    refreshCartCount();
    updatePlaceOrderBtn();

    // BẮT SỰ KIỆN SUBMIT checkoutForm với login-check
    const checkoutForm = document.getElementById('checkoutForm');
    if (checkoutForm) {
        checkoutForm.addEventListener('submit', e => {
            // nếu chưa login thì chặn submit và alert
            if (!window.isAuthenticated) {
                e.preventDefault();
                alert('Bạn cần đăng nhập trước khi đặt hàng.');
                // redirect đến login page nếu muốn
                window.location.href = '/Authen/Login_Register';
                return;
            }
            // nếu đã login, inject JSON cart
            const cartJson = JSON.stringify(getCart());
            document.getElementById('CartData').value = cartJson;
        });
    }

    document.querySelectorAll('.cart-icon').forEach(btn => {
        btn.addEventListener('click', () => {
            const pid = parseInt(btn.dataset.productId, 10);
            const name = btn.dataset.productName;
            const price = parseFloat(btn.dataset.price);
            const img = btn.dataset.imageUrl;

            const qtyInput = btn.closest('ul')?.querySelector('input[type="number"]');
            const qty = qtyInput ? parseInt(qtyInput.value, 10) || 1 : 1;

            if (qty > 20) {
                alert('❌ Bạn đang mua quá nhiều. Vui lòng chọn tối đa 20 sản phẩm.');
                qtyInput.value = 20; 
                return;
            }

            addToCart(pid, name, price, img, qty);
            showAddToast();
        });
    });



    document.body.addEventListener('click', e => {
        if (e.target.matches('.remove-btn')) {
            const pid = parseInt(e.target.dataset.pid, 10);
            removeFromCart(pid);
        }
    });

    if (document.getElementById('cartBody')) {
        renderCartTable();
    }
});

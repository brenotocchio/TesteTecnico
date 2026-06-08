const API = 'http://localhost:5000/api';

// Verifica se está logado.
function verificarLogin() {
  if (!sessionStorage.getItem('logado')) {
    window.location.href = 'index.html';
  }
}

// Desloga e volta para o login
function sair() {
  sessionStorage.removeItem('logado');
  window.location.href = 'index.html';
}

// Requisições para a API
async function api(method, url, body = null) {
  const opts = {
    method,
    headers: { 'Content-Type': 'application/json' }
  };
  if (body) opts.body = JSON.stringify(body);
  const res = await fetch(API + url, opts);
  return res.json();
}

// Exibe uma mensagem toast na tela
function toast(msg, tipo = 'verde') {
  const t = document.getElementById('toast');
  t.textContent = msg;
  t.className = tipo;
  t.style.display = 'block';
  setTimeout(() => t.style.display = 'none', 3000);
}

// Máscara decimal (casas = quantidade de casas decimais)
function mascaraDecimal(input, casas) {
  input.addEventListener('input', function () {
    let v = this.value.replace(/\D/g, '');
    if (!v) { this.value = ''; return; }
    v = (parseInt(v) / Math.pow(10, casas)).toFixed(casas);
    this.value = v.replace('.', ',');
  });
}

// Máscara CPF/CNPJ automática
function mascaraDocumento(input) {
  input.addEventListener('input', function () {
    let v = this.value.replace(/\D/g, '');
    if (v.length <= 11) {
      v = v.replace(/(\d{3})(\d)/, '$1.$2')
           .replace(/(\d{3})(\d)/, '$1.$2')
           .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    } else {
      v = v.replace(/^(\d{2})(\d)/, '$1.$2')
           .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
           .replace(/\.(\d{3})(\d)/, '.$1/$2')
           .replace(/(\d{4})(\d)/, '$1-$2');
    }
    this.value = v.slice(0, 18);
  });
}

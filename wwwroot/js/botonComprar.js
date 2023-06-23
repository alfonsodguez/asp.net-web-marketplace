$('a[id^="btnComprar"]').click(function () {
    const ean = $(this).attr('id').split('-')[1];
    const cantidad = parseFloat($(`label[id*="${ean}"`).text())

    const infoProducto = {
        cantidadPedido: cantidad,
        productoPedido: {
            ean
        }
    }

    $.ajax({
        type: 'POST',
        url: 'https://localhost:44329/Pedido/AddCestaProducto',
        data: JSON.stringify(infoProducto),
        contentType: 'application/json',
        processData: false
    })
    .done((res) => {
        console.log('producto añadido al carrito ok')
    })
    .fail((err) => {
        console.log('fallo al añadir producto al carrito')
    })
})

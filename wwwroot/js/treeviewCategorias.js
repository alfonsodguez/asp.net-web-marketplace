const treeview = $('#treeviewCat')
const separadorCategoria = '-' 

categorias.forEach((categoria) => {
    const paths = categoria.path.split(separadorCategoria)
    if (paths.length === 1) {
        treeview.append(`<li id="${categoria.path}"><span class="caret">${categoria.nombre}</span></li>`)
    }
})

function despliegaSubcategorias() {
    const idCategoria = $(this).parent().attr('id')
    const reCategoria = new RegExp("^" + idCategoria + "-[0-9]+$")
    const subCategorias = categorias.filter(categoria => categoria.path.search(reCategoria) != -1)

    if (subCategorias.length > 0) {
        let tags = '<ul class="nested">'

        subCategorias.forEach((subcategoria) => {
            tags += `<li id="${subcategoria.path}"><span class="caret">${subcategoria.nombre}</span></li>`
        })
        tags += '</ul>'

        $(this).parent().append(tags)
        $(".caret").click(despliegaSubcategorias)
    } else {
        const urlProductos = "https://localhost:44329/Tienda/Productos/" + idCategoria
        window.location = urlProductos
    }

    $(this).parent().children("ul.nested").toggleClass("active")
    $(this).toggleClass("caret-down")
}

$(".caret").click(despliegaSubcategorias)
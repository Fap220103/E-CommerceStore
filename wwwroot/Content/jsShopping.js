

$(document).ready(function () {
 
    showCount();
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();      
        var id = $(this).data('id');
        var quantity = parseInt($('#quantity_value').text()) || 1;
     
        $.ajax({
            url: '/shoppingcart/addtocart',
            type: 'POST',
            data: { id: id, quantity: quantity },
            success: function (rs) {
                //console.log('Response:', rs);
                if (rs.success) {
                    $('#checkout_items').html(rs.count);
                   notyf.success("Thêm sản phẩm vào giỏ hàng thành công");
                } else {
                   notyf.error("Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng");
                }
            }
        });
    });

    $('body').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var conf = confirm("Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng không?");
        if (conf) {
            $.ajax({
                url: '/shoppingcart/delete',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                   // console.log(rs);
                    if (rs.success) {
                        $('#checkout_items').html(rs.count);
                        $('#trow_' + id).remove();
                        //loadCart();
                        notyf.success("Xóa sản phẩm thành công");
                    } else {
                        notyf.error("Xóa sản phẩm thất bại");
                    }
                }
            });
        }
    });

    $('body').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
        var conf = confirm("Bạn có chắc chắn muốn xóa hết sản phẩm khỏi giỏ hàng không?");
        if (conf) {
           
            deleteAll();
           
        }
    });

    $('body').on('click', '.btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = $('#Quantity_' + id).val();
        updateCart(id, quantity);
    });
});

function showCount() {
    $.ajax({
        url: '/shoppingcart/Show_Count',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.count);
        }
    });
}

function deleteAll() {
    $.ajax({
        url: '/shoppingcart/DeleteAll',
        type: 'POST',
        success: function (rs) {
           // console.log(rs);
            if (rs.success) {
                loadCart();
                $('#checkout_items').html('0');
                notyf.success("Xóa thành công");
            } else {
                notyf.error("Xóa tất cả sản phẩm thất bại");
            }
        }
    });
}

function updateCart(id, quantity) {
    $.ajax({
        url: '/shoppingcart/Update',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (rs) {
         //   console.log(rs);
            if (rs.success) {
                loadCart();
                notyf.success("Cập nhật giỏ hàng thành công");
            } else {
                notyf.error("Cập nhật giỏ hàng thất bại");
            }
        }
    });
}

function loadCart() {
    $.ajax({
        url: '/shoppingcart/Partial_Item_Cart',
        type: 'GET',
        success: function (rs) {
          //  console.log(rs);
            $('#load_data').html(rs);
        }
    });
}


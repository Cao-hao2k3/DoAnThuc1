document.addEventListener("DOMContentLoaded", function () {
    const provinceSelect = document.getElementById("provinceSelect");
    const districtSelect = document.getElementById("districtSelect");
    const wardSelect = document.getElementById("wardSelect");

    // Fetch danh sách tỉnh/thành
    fetch("https://provinces.open-api.vn/api/?depth=1")
        .then(response => response.json())
        .then(data => {
            data.forEach(province => {
                let option = new Option(province.name, province.code);
                provinceSelect.add(option);
            });
        });

    // Khi chọn tỉnh/thành, tải quận/huyện tương ứng
    provinceSelect.addEventListener("change", function () {
        let provinceCode = this.value;
        districtSelect.innerHTML = '<option value="">Chọn quận/huyện</option>';
        wardSelect.innerHTML = '<option value="">Chọn phường/xã</option>';
        wardSelect.disabled = true;

        if (!provinceCode) {
            districtSelect.disabled = true;
            return;
        }

        fetch(`https://provinces.open-api.vn/api/p/${provinceCode}?depth=2`)
            .then(response => response.json())
            .then(data => {
                data.districts.forEach(district => {
                    let option = new Option(district.name, district.code);
                    districtSelect.add(option);
                });
                districtSelect.disabled = false;
            });
    });

    // Khi chọn quận/huyện, tải phường/xã tương ứng
    districtSelect.addEventListener("change", function () {
        let districtCode = this.value;
        wardSelect.innerHTML = '<option value="">Chọn phường/xã</option>';

        if (!districtCode) {
            wardSelect.disabled = true;
            return;
        }

        fetch(`https://provinces.open-api.vn/api/d/${districtCode}?depth=2`)
            .then(response => response.json())
            .then(data => {
                data.wards.forEach(ward => {
                    let option = new Option(ward.name, ward.code);
                    wardSelect.add(option);
                });
                wardSelect.disabled = false;
            });
    });
});

// Toggle giữa "Giao tận nơi" và "Nhận tại siêu thị"
document.querySelectorAll('.btn-toggle').forEach(button => {
    button.addEventListener('click', function () {
        document.querySelectorAll('.btn-toggle').forEach(btn => btn.classList.remove('active'));
        this.classList.add('active');
    });
});

// Cập nhật địa chỉ khi nhấn "Xác Nhận"
document.getElementById('confirmAddress').addEventListener('click', function () {
    const newAddress = document.getElementById('provinceInput').value + ', ' + document.getElementById('addressInput').value;
    document.getElementById('currentAddress').innerHTML = '📍 ' + newAddress;
    new bootstrap.Modal(document.getElementById('addressModal')).hide();
});
//Hiển thị form nếu người khác nhận
document.getElementById("otherReceiver").addEventListener("change", function () {
    const form = document.getElementById("otherReceiverForm");
    if (this.checked) {
        form.style.display = "block"; // Hiển thị form
    } else {
        form.style.display = "none";  // Ẩn form
    }
});

document.getElementById("otherReceiver").addEventListener("change", function () {
    const form = document.getElementById("otherReceiverForm");
    form.style.display = this.checked ? "block" : "none";
});

    
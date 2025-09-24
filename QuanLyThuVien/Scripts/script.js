document.addEventListener('DOMContentLoaded', function () {
    // Lấy form tìm kiếm bằng ID
    const searchForm = document.getElementById('search-form');

    // Kiểm tra xem form có tồn tại không
    if (searchForm) {
        // Gắn một sự kiện "submit" cho form
        searchForm.addEventListener('submit', function (event) {
            // Ngăn chặn hành động mặc định của form (tải lại trang)
            event.preventDefault();

            // Lấy giá trị người dùng nhập vào ô tên sách
            const tenSach = document.getElementById('ten-sach').value;

            // Hiển thị một thông báo đơn giản
            if (tenSach) {
                alert('Đang tìm kiếm sách với từ khóa: "' + tenSach + '"\n(Đây là giao diện tĩnh, chức năng tìm kiếm thật cần back-end.)');
            } else {
                alert('Vui lòng nhập tên sách để tìm kiếm!');
            }
        });
    }
});
// --- LOGIC CHO CỬA SỔ ĐĂNG NHẬP (MODAL) ---
document.addEventListener('DOMContentLoaded', function () {
    // ... (Giữ nguyên code lấy phần tử và mở/đóng modal) ...
    const loginModal = document.getElementById('login-modal');
    const loginTriggerBtn = document.getElementById('login-trigger-btn');
    const closeBtn = document.querySelector('.close-btn');
    const modalLoginForm = document.getElementById('modal-login-form');

    function openModal() { if (loginModal) loginModal.classList.add('show-modal'); }
    function closeModal() { if (loginModal) loginModal.classList.remove('show-modal'); }

    if (loginTriggerBtn) loginTriggerBtn.addEventListener('click', openModal);
    if (closeBtn) closeBtn.addEventListener('click', closeModal);
    if (loginModal) loginModal.addEventListener('click', function (event) { if (event.target === loginModal) closeModal(); });

    // Xử lý sự kiện submit form đăng nhập
    if (modalLoginForm) {
        modalLoginForm.addEventListener('submit', function (event) {
            event.preventDefault(); // Ngăn trang tải lại

            var formData = $(this).serialize();

            $.ajax({
                url: '/Account/Login', // URL đến Action Login
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response.success) {
                        alert('Đăng nhập thành công!');
                        // Chuyển hướng đến trang được trả về từ server
                        window.location.href = response.redirectUrl;
                    } else {
                        alert('Lỗi: ' + response.message);
                    }
                },
                error: function () {
                    alert('Không thể kết nối đến server. Vui lòng thử lại.');
                }
            });
        });
    }
});

// --- LOGIC CHO CỬA SỔ ĐĂNG KÝ (MODAL) ---
document.addEventListener('DOMContentLoaded', function () {
    // ... (Giữ nguyên code lấy phần tử và chuyển đổi giữa các modal) ...
    const signupModal = document.getElementById('signup-modal');
    const closeBtnSignup = document.querySelector('.close-btn-signup');
    const modalSignupForm = document.getElementById('modal-signup-form');
    const showSignupLink = document.getElementById('show-signup-modal');
    const showLoginLink = document.getElementById('show-login-modal');
    const loginModal = document.getElementById('login-modal');

    function openSignupModal() { if (signupModal) signupModal.classList.add('show-modal'); }
    function closeSignupModal() { if (signupModal) signupModal.classList.remove('show-modal'); }
    function closeLoginModal() { if (loginModal) loginModal.classList.remove('show-modal'); }
    function openLoginModal() { if (loginModal) loginModal.classList.add('show-modal'); }

    if (showSignupLink) { showSignupLink.addEventListener('click', function (e) { e.preventDefault(); closeLoginModal(); openSignupModal(); }); }
    if (showLoginLink) { showLoginLink.addEventListener('click', function (e) { e.preventDefault(); closeSignupModal(); openLoginModal(); }); }
    if (closeBtnSignup) closeBtnSignup.addEventListener('click', closeSignupModal);
    if (signupModal) { signupModal.addEventListener('click', function (e) { if (e.target === signupModal) closeSignupModal(); }); }

    // Xử lý sự kiện submit form đăng ký
    if (modalSignupForm) {
        modalSignupForm.addEventListener('submit', function (event) {
            event.preventDefault(); // Ngăn trang tải lại

            // Kiểm tra mật khẩu xác nhận phía client
            const password = document.getElementById('signup-password').value;
            const confirmPassword = document.getElementById('signup-confirm-password').value;
            if (password !== confirmPassword) {
                alert('Mật khẩu xác nhận không khớp!');
                return;
            }

            var formData = $(this).serialize();

            $.ajax({
                url: '/Account/Register', // URL đến Action Register
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        closeSignupModal(); // Đóng modal đăng ký
                        openLoginModal();   // Mở modal đăng nhập để người dùng đăng nhập
                    } else {
                        alert('Lỗi: ' + response.message);
                    }
                },
                error: function () {
                    alert('Không thể kết nối đến server. Vui lòng thử lại.');
                }
            });
        });
    }
});
// --- LOGIC FOR QUAN-LY-SACH.HTML (PHIÊN BẢN HOÀN CHỈNH) ---
// --- LOGIC FOR QUAN-LY-SACH.HTML (PHIÊN BẢN ĐÃ SỬA LỖI) ---

// Sử dụng cú pháp $(document).ready() của jQuery để đảm bảo mọi thứ đã sẵn sàng
$(document).ready(function () {
    const bookModal = document.getElementById('book-modal');
    // Chỉ chạy code này nếu có modal sách trên trang
    if (!bookModal) return;


    // Khai báo tất cả các phần tử cần thiết MỘT LẦN DUY NHẤT
    const addBookBtn = document.getElementById('add-book-btn');
    const closeBookModalBtn = document.querySelector('.close-book-modal');
    const bookModalTitle = document.getElementById('book-modal-title');
    const bookForm = document.getElementById('book-form');
    // KHAI BÁO THÊM BIẾN NÀY: Tham chiếu đến input ẩn chứa MaSach
    const hiddenBookId = $('#ma-sach-modal');


    // --- CÁC HÀM HỖ TRỢ ---
    function openBookModal() {
        bookModal.classList.add('show-modal');
    }


    function closeBookModal() {
        bookModal.classList.remove('show-modal');
    }

    // BỔ SUNG HÀM NÀY: Hàm để reset form về trạng thái ban đầu
    function resetForm() {
        bookForm.reset(); // Xóa dữ liệu trên các trường input thông thường
        hiddenBookId.val(''); // Quan trọng: Xóa ID sách đang được lưu trong trường ẩn
        // Dòng dưới để reset ListBox đa lựa chọn (nếu bạn dùng plugin như Select2 thì nó hữu ích)
        $('#the-loai').val(null);
    }


    // --- GẮN CÁC SỰ KIỆN ---


    // Sự kiện khi bấm nút "Thêm Sách Mới"
    $(addBookBtn).on('click', function () {
        bookModalTitle.textContent = "Thêm Sách Mới";
        // SỬA LẠI: Dùng hàm resetForm() để đảm bảo trường ẩn cũng được xóa
        resetForm();
        openBookModal();
    });


    // Sự kiện khi bấm nút "Sửa"
    $('#sach-table').on('click', '.btn-edit', function () {
        const bookId = $(this).data('id');


        // Gọi AJAX để lấy thông tin chi tiết của sách
        $.ajax({
            url: '/Book/GetBookDetails',
            type: 'GET',
            data: { id: bookId },
            success: function (response) {
                if (response.success) {
                    const data = response.data;
                    const sach = data.sach;


                    // Điền thông tin vào form
                    resetForm(); // Reset trước khi điền để đảm bảo form sạch
                    bookModalTitle.textContent = 'Chỉnh sửa Thông tin Sách';


                    hiddenBookId.val(sach.MaSach);
                    $('#ten-sach-modal').val(sach.TenSach);
                    $('#so-luong').val(sach.SoLuongTon);
                    $('#tac-gia').val(sach.MaTacGia);
                    $('#nxb').val(sach.MaNXB);
                    $('#mo-ta').val(sach.MoTa);


                    // Chọn các thể loại tương ứng trong ListBox
                    $('#the-loai').val(data.selectedTheLoaiIds);


                    openBookModal(); // Mở modal sau khi đã điền xong dữ liệu
                } else {
                    alert('Lỗi: ' + response.message);
                }
            },
            error: function () {
                alert('Không thể kết nối đến server để lấy dữ liệu sách.');
            }
        });
    });


    // LOGIC NÚT XÓA (Giữ nguyên)
    $('#sach-table').on('click', '.btn-delete', function () {
        const button = $(this);
        const bookId = button.data('id');
        const bookName = button.closest('tr').find('td:eq(1)').text();


        if (confirm(`Bạn có chắc chắn muốn xóa sách "${bookName}" không?`)) {
            $.ajax({
                url: '/Book/Delete',
                type: 'POST',
                data: { id: bookId },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        button.closest('tr').fadeOut(500, function () {
                            $(this).remove();
                        });
                    } else {
                        alert('Lỗi: ' + response.message);
                    }
                },
                error: function () {
                    alert('Không thể kết nối đến server. Vui lòng thử lại.');
                }
            });
        }
    });


    // Các sự kiện đóng modal
    $(closeBookModalBtn).on('click', closeBookModal);
    $(bookModal).on('click', function (event) {
        if (event.target === bookModal) {
            closeBookModal();
        }
    });


    // XỬ LÝ SUBMIT FORM CHO CẢ THÊM VÀ SỬA
    $('#book-form').on('submit', function (event) {
        event.preventDefault();


        if (!this.checkValidity()) {
            this.reportValidity();
            return;
        }


        const bookId = hiddenBookId.val();
        // Quyết định URL sẽ gửi đến dựa trên việc có bookId hay không
        const submissionUrl = bookId ? '/Book/Edit' : '/Book/Create';

        var formData = $(this).serialize();


        $.ajax({
            url: submissionUrl,
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    closeBookModal();
                    location.reload(); // Tải lại trang để thấy dữ liệu mới/đã cập nhật
                } else {
                    alert('Lỗi: ' + response.message);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("AJAX Error:", textStatus, errorThrown, jqXHR.responseText);
                alert('Không thể kết nối đến server. Vui lòng kiểm tra console (F12).');
            }
        });
    });
});

// --- LOGIC FOR QUAN-LY-DOC-GIA.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    const readerModal = document.getElementById('reader-modal');
    if (!readerModal) return;

    const addReaderBtn = document.getElementById('add-reader-btn');
    const closeReaderModalBtn = document.querySelector('.close-reader-modal');
    const readerModalTitle = document.getElementById('reader-modal-title');
    const readerForm = document.getElementById('reader-form');

    function openReaderModal() {
        readerModal.classList.add('show-modal');
    }

    function closeReaderModal() {
        readerModal.classList.remove('show-modal');
    }

    addReaderBtn.addEventListener('click', function () {
        readerModalTitle.textContent = "Thêm Độc giả Mới";
        readerForm.reset();
        document.getElementById('ma-doc-gia').readOnly = false;
        openReaderModal();
    });

    document.querySelectorAll('.data-table-section .btn-edit').forEach(button => {
        button.addEventListener('click', function () {
            if (this.closest('tr').cells[0].textContent.startsWith('DG')) {
                readerModalTitle.textContent = "Chỉnh sửa Thông tin Độc giả";

                const row = this.closest('tr');
                document.getElementById('ma-doc-gia').value = row.cells[0].textContent;
                document.getElementById('ma-doc-gia').readOnly = true;
                document.getElementById('ho-ten').value = row.cells[1].textContent;
                document.getElementById('lop').value = row.cells[2].textContent;
                document.getElementById('email').value = row.cells[3].textContent;

                openReaderModal();
            }
        });
    });

    document.querySelectorAll('.data-table-section .btn-delete').forEach(button => {
        button.addEventListener('click', function () {
            if (this.closest('tr').cells[0].textContent.startsWith('DG')) {
                if (confirm('Bạn có chắc chắn muốn xóa độc giả này không?')) {
                    this.closest('tr').remove();
                    alert('Xóa độc giả thành công! (Mô phỏng)');
                }
            }
        });
    });

    closeReaderModalBtn.addEventListener('click', closeReaderModal);
    readerModal.addEventListener('click', function (event) {
        if (event.target === readerModal) {
            closeReaderModal();
        }
    });

    readerForm.addEventListener('submit', function (event) {
        event.preventDefault();
        alert('Lưu thông tin độc giả thành công! (Mô phỏng)');
        closeReaderModal();
    });
});
// --- LOGIC FOR QUAN-LY-MUON-TRA.HTML (VERSION 2 - UPDATED) ---

document.addEventListener('DOMContentLoaded', function () {
    const loanModal = document.getElementById('loan-modal');
    if (!loanModal) return;

    const addLoanBtn = document.getElementById('add-loan-btn');
    const closeLoanModalBtns = document.querySelectorAll('.close-loan-modal, .close-loan-modal-btn');
    const loanModalTitle = document.getElementById('loan-modal-title');
    const loanForm = document.getElementById('loan-form');
    const maPhieuInput = document.getElementById('ma-phieu');
    const addBookToListBtn = document.getElementById('add-book-to-list-btn');
    const sachSelect = document.getElementById('sach-select');
    const selectedBooksList = document.getElementById('selected-books-list');
    const dataTableBody = document.querySelector('.data-table-section tbody');

    // Hàm điều khiển trạng thái form (bật/tắt)
    function setFormEnabled(enabled) {
        loanForm.querySelectorAll('input, select, textarea, button').forEach(el => {
            el.disabled = !enabled;
        });
        // Luôn bật các nút đóng modal
        document.querySelectorAll('.close-loan-modal, .close-loan-modal-btn').forEach(btn => btn.disabled = false);
    }

    function openLoanModal() {
        loanModal.classList.add('show-modal');
    }

    function closeLoanModal() {
        loanModal.classList.remove('show-modal');
    }

    // ACTION: Mở modal để TẠO PHIẾU MƯỢN MỚI
    addLoanBtn.addEventListener('click', function () {
        loanModalTitle.textContent = "Tạo Phiếu Mượn Mới";
        loanForm.reset();
        selectedBooksList.innerHTML = '';
        maPhieuInput.value = "PM" + Math.floor(1000 + Math.random() * 9000);
        setFormEnabled(true); // Bật form để nhập liệu
        openLoanModal();
    });

    // SỬ DỤNG EVENT DELEGATION ĐỂ XỬ LÝ CLICK TRÊN TOÀN BỘ BẢNG
    dataTableBody.addEventListener('click', function (event) {
        const target = event.target.closest('button');
        if (!target) return; // Bỏ qua nếu không click vào button

        const row = target.closest('tr');

        // ACTION: Mở modal để XEM CHI TIẾT
        if (target.classList.contains('btn-view-details')) {
            loanModalTitle.textContent = "Chi tiết Phiếu Mượn - " + row.cells[0].textContent;
            loanForm.reset();
            // Mô phỏng việc điền dữ liệu vào form
            document.getElementById('doc-gia-select').value = 'DG001'; // Giả sử là DG001
            document.getElementById('ngay-muon').value = '2025-09-12';
            document.getElementById('ngay-hen-tra').value = '2025-09-26';
            selectedBooksList.innerHTML = `<li><span>S0123 - Lập trình Hướng đối tượng</span></li>`;

            setFormEnabled(false); // Tắt form, chỉ cho xem
            openLoanModal();
        }

        // ACTION: ĐÁNH DẤU TRẢ SÁCH
        if (target.classList.contains('btn-mark-returned')) {
            if (confirm('Xác nhận độc giả đã trả sách cho phiếu mượn [' + row.cells[0].textContent + ']?')) {
                const statusCell = row.querySelector('.status');
                statusCell.className = 'status returned';
                statusCell.textContent = 'Đã trả';
                target.remove(); // Xóa nút "Trả sách" sau khi đã trả
                alert('Cập nhật trạng thái thành công!');
            }
        }

        // ACTION: XÓA PHIẾU MƯỢN
        if (target.classList.contains('btn-delete')) {
            if (confirm('Bạn có chắc chắn muốn xóa vĩnh viễn phiếu mượn [' + row.cells[0].textContent + ']?')) {
                row.remove();
                alert('Xóa phiếu mượn thành công!');
            }
        }
    });


    // Các sự kiện đóng modal
    closeLoanModalBtns.forEach(btn => btn.addEventListener('click', closeLoanModal));
    loanModal.addEventListener('click', function (event) {
        if (event.target === loanModal) {
            closeLoanModal();
        }
    });

    // Sự kiện submit form
    loanForm.addEventListener('submit', function (event) {
        event.preventDefault();
        alert('Lưu thông tin phiếu mượn thành công! (Mô phỏng)');
        closeLoanModal();
    });

    // Logic thêm sách vào danh sách trong modal
    addBookToListBtn.addEventListener('click', function () {
        const selectedOption = sachSelect.options[sachSelect.selectedIndex];
        if (selectedOption.value) {
            const bookId = selectedOption.value;
            const bookName = selectedOption.text;

            if (document.querySelector(`li[data-book-id="${bookId}"]`)) {
                alert('Sách này đã được thêm vào danh sách!');
                return;
            }
            const li = document.createElement('li');
            li.setAttribute('data-book-id', bookId);
            li.innerHTML = `<span>${bookName}</span><button type="button" class="remove-book-btn">&times;</button>`;
            selectedBooksList.appendChild(li);
        }
    });

    // Logic xóa sách khỏi danh sách
    selectedBooksList.addEventListener('click', function (event) {
        if (event.target.classList.contains('remove-book-btn')) {
            event.target.closest('li').remove();
        }
    });
});
// --- LOGIC FOR BAO-CAO-THONG-KE.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    // Chỉ chạy code này nếu tìm thấy canvas của biểu đồ trên trang
    const loanChartCanvas = document.getElementById('loanChart');
    if (loanChartCanvas) {

        // --- Biểu đồ 1: Lượt mượn sách (Line Chart) ---
        const ctxLoan = loanChartCanvas.getContext('2d');
        new Chart(ctxLoan, {
            type: 'line',
            data: {
                labels: ['Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9'],
                datasets: [{
                    label: 'Lượt mượn',
                    data: [850, 920, 1100, 1050, 1210, 1280],
                    backgroundColor: 'rgba(0, 123, 255, 0.2)',
                    borderColor: 'rgba(0, 123, 255, 1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.4 // Làm cho đường cong mượt hơn
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });

        // --- Biểu đồ 2: Cơ cấu thể loại (Doughnut Chart) ---
        const categoryChartCanvas = document.getElementById('categoryChart').getContext('2d');
        new Chart(categoryChartCanvas, {
            type: 'doughnut',
            data: {
                labels: ['Khoa học Máy tính', 'Kỹ thuật Xây dựng', 'Kinh tế', 'Ngoại ngữ', 'Văn học'],
                datasets: [{
                    label: 'Số lượng sách',
                    data: [4500, 3200, 2500, 1560, 800],
                    backgroundColor: [
                        '#007bff',
                        '#28a745',
                        '#ffc107',
                        '#dc3545',
                        '#6c757d'
                    ],
                    hoverOffset: 4
                }]
            },
            options: {
                responsive: true,
            }
        });

        // --- Xử lý sự kiện cho nút Xuất Báo cáo ---
        const exportBtn = document.getElementById('export-report-btn');
        if (exportBtn) {
            exportBtn.addEventListener('click', function () {
                alert('Chức năng đang được phát triển! Dữ liệu sẽ được xuất ra file Excel. (Mô phỏng)');
            });
        }
    }
});
// --- LOGIC FOR QUAN-LY-NGUOI-DUNG.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    // Chỉ chạy code này nếu đang ở trang Quản lý Người dùng
    const userPageIdentifier = document.getElementById('add-user-btn');
    if (!userPageIdentifier) return;

    // Lấy các phần tử DOM
    const userModal = document.getElementById('user-modal');
    const addUserBtn = document.getElementById('add-user-btn');
    const closeUserModalBtn = document.querySelector('.close-user-modal');
    const userModalTitle = document.getElementById('user-modal-title');
    const userForm = document.getElementById('user-form');
    const dataTableBody = document.querySelector('.data-table-section tbody');

    function openUserModal() {
        if (userModal) userModal.classList.add('show-modal');
    }

    function closeUserModal() {
        if (userModal) userModal.classList.remove('show-modal');
    }

    // ACTION 1: Mở modal để THÊM NGƯỜI DÙNG MỚI
    addUserBtn.addEventListener('click', function () {
        userModalTitle.textContent = "Thêm Người dùng Mới";
        userForm.reset();
        document.getElementById('username').readOnly = false;
        openUserModal();
    });

    // SỬ DỤNG EVENT DELEGATION cho các nút Sửa và Xóa
    dataTableBody.addEventListener('click', function (event) {
        const buttonClicked = event.target.closest('button');
        if (!buttonClicked) return;

        const row = buttonClicked.closest('tr');
        const username = row.cells[0].textContent;

        // ACTION 2: Xử lý nút SỬA
        if (buttonClicked.classList.contains('btn-edit')) {
            userModalTitle.textContent = "Chỉnh sửa Người dùng";

            // Điền dữ liệu từ bảng vào form
            document.getElementById('username').value = username;
            document.getElementById('username').readOnly = true; // Không cho sửa username
            document.getElementById('full-name').value = row.cells[1].textContent;
            document.getElementById('email-user').value = row.cells[2].textContent;
            // (Thêm code để chọn đúng role và status trong select)

            openUserModal();
        }

        // ACTION 3: Xử lý nút XÓA
        if (buttonClicked.classList.contains('btn-delete')) {
            if (confirm(`Bạn có chắc chắn muốn xóa người dùng [${username}]?`)) {
                row.remove();
                alert('Xóa người dùng thành công!');
            }
        }
    });

    // Sự kiện đóng modal
    if (closeUserModalBtn) closeUserModalBtn.addEventListener('click', closeUserModal);
    if (userModal) {
        userModal.addEventListener('click', function (event) {
            if (event.target === userModal) closeUserModal();
        });
    }

    // Sự kiện submit form
    if (userForm) {
        userForm.addEventListener('submit', function (event) {
            event.preventDefault();
            alert('Lưu thông tin người dùng thành công! (Mô phỏng)');
            closeUserModal();
        });
    }
});

// --- LOGIC FOR QUAN-LY-NXB.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    const nxbPageIdentifier = document.getElementById('add-nxb-btn');
    if (!nxbPageIdentifier) return;

    const nxbModal = document.getElementById('nxb-modal');
    const addNxbBtn = document.getElementById('add-nxb-btn');
    const closeNxbModalBtn = document.querySelector('.close-nxb-modal');
    const nxbModalTitle = document.getElementById('nxb-modal-title');
    const nxbForm = document.getElementById('nxb-form');
    const dataTableBody = document.querySelector('.data-table-section tbody');

    function openNxbModal() { if (nxbModal) nxbModal.classList.add('show-modal'); }
    function closeNxbModal() { if (nxbModal) nxbModal.classList.remove('show-modal'); }

    addNxbBtn.addEventListener('click', function () {
        nxbModalTitle.textContent = "Thêm Nhà Xuất Bản Mới";
        nxbForm.reset();
        document.getElementById('ma-nxb').readOnly = false;
        openNxbModal();
    });

    dataTableBody.addEventListener('click', function (event) {
        const button = event.target.closest('button');
        if (!button) return;
        const row = button.closest('tr');
        const maNXB = row.cells[0].textContent;

        if (button.classList.contains('btn-edit')) {
            nxbModalTitle.textContent = "Chỉnh sửa Nhà Xuất Bản";
            document.getElementById('ma-nxb').value = maNXB;
            document.getElementById('ma-nxb').readOnly = true;
            document.getElementById('ten-nxb').value = row.cells[1].textContent;
            document.getElementById('dia-chi').value = row.cells[2].textContent;
            document.getElementById('email-nxb').value = row.cells[3].textContent;
            openNxbModal();
        }

        if (button.classList.contains('btn-delete')) {
            if (confirm(`Bạn có chắc chắn muốn xóa NXB [${maNXB}]?`)) {
                row.remove();
                alert('Xóa NXB thành công!');
            }
        }
    });

    if (closeNxbModalBtn) closeNxbModalBtn.addEventListener('click', closeNxbModal);
    if (nxbModal) nxbModal.addEventListener('click', e => (e.target === nxbModal) && closeNxbModal());
    if (nxbForm) nxbForm.addEventListener('submit', function (e) {
        e.preventDefault();
        alert('Lưu thông tin NXB thành công!');
        closeNxbModal();
    });
});

// --- LOGIC FOR QUAN-LY-TAC-GIA.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    const authorPageIdentifier = document.getElementById('add-author-btn');
    if (!authorPageIdentifier) return;

    const authorModal = document.getElementById('author-modal');
    const addAuthorBtn = document.getElementById('add-author-btn');
    const closeAuthorModalBtn = document.querySelector('.close-author-modal');
    const authorModalTitle = document.getElementById('author-modal-title');
    const authorForm = document.getElementById('author-form');
    const dataTableBody = document.querySelector('.data-table-section tbody');

    function openAuthorModal() { if (authorModal) authorModal.classList.add('show-modal'); }
    function closeAuthorModal() { if (authorModal) authorModal.classList.remove('show-modal'); }

    addAuthorBtn.addEventListener('click', function () {
        authorModalTitle.textContent = "Thêm Tác giả Mới";
        authorForm.reset();
        document.getElementById('ma-tac-gia').readOnly = false;
        openAuthorModal();
    });

    dataTableBody.addEventListener('click', function (event) {
        const button = event.target.closest('button');
        if (!button) return;
        const row = button.closest('tr');
        const maTacGia = row.cells[0].textContent;

        if (button.classList.contains('btn-edit')) {
            authorModalTitle.textContent = "Chỉnh sửa Tác giả";
            document.getElementById('ma-tac-gia').value = maTacGia;
            document.getElementById('ma-tac-gia').readOnly = true;
            document.getElementById('ten-tac-gia').value = row.cells[1].textContent;
            document.getElementById('thong-tin-lien-he').value = row.cells[2].textContent;
            openAuthorModal();
        }

        if (button.classList.contains('btn-delete')) {
            if (confirm(`Bạn có chắc chắn muốn xóa tác giả [${maTacGia}]?`)) {
                row.remove();
                alert('Xóa tác giả thành công!');
            }
        }
    });

    if (closeAuthorModalBtn) closeAuthorModalBtn.addEventListener('click', closeAuthorModal);
    if (authorModal) authorModal.addEventListener('click', e => (e.target === authorModal) && closeAuthorModal());
    if (authorForm) authorForm.addEventListener('submit', function (e) {
        e.preventDefault();
        alert('Lưu thông tin tác giả thành công!');
        closeAuthorModal();
    });
});

// --- LOGIC FOR QUAN-LY-VI-PHAM.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    // Chỉ chạy code này nếu đang ở trang Quản lý Vi phạm
    const violationPageIdentifier = document.getElementById('add-violation-btn');
    if (!violationPageIdentifier) return;

    // Lấy các phần tử DOM
    const violationModal = document.getElementById('violation-modal');
    const addViolationBtn = document.getElementById('add-violation-btn');
    const closeViolationModalBtn = document.querySelector('.close-violation-modal');
    const violationModalTitle = document.getElementById('violation-modal-title');
    const violationForm = document.getElementById('violation-form');
    const dataTableBody = document.querySelector('.data-table-section tbody');

    function openViolationModal() { if (violationModal) violationModal.classList.add('show-modal'); }
    function closeViolationModal() { if (violationModal) violationModal.classList.remove('show-modal'); }

    // ACTION 1: Mở modal để LẬP PHIẾU PHẠT MỚI
    addViolationBtn.addEventListener('click', function () {
        violationModalTitle.textContent = "Lập Phiếu Phạt Mới";
        violationForm.reset();
        document.getElementById('ma-vi-pham').value = "VP" + Math.floor(100 + Math.random() * 900);
        openViolationModal();
    });

    // SỬ DỤNG EVENT DELEGATION cho các nút trong bảng
    dataTableBody.addEventListener('click', function (event) {
        const button = event.target.closest('button');
        if (!button) return;
        const row = button.closest('tr');
        const maVP = row.cells[0].textContent;

        // ACTION 2: Xử lý nút "ĐÃ TRẢ"
        if (button.classList.contains('btn-mark-paid')) {
            if (confirm(`Xác nhận độc giả đã thanh toán khoản phạt cho phiếu [${maVP}]?`)) {
                const statusCell = row.querySelector('.status');
                statusCell.className = 'status paid';
                statusCell.textContent = 'Đã thanh toán';
                button.remove(); // Xóa nút "Đã trả" sau khi đã xác nhận
                alert('Cập nhật trạng thái thành công!');
            }
        }

        // ACTION 3: Xử lý nút "XÓA"
        if (button.classList.contains('btn-delete')) {
            if (confirm(`Bạn có chắc chắn muốn xóa phiếu phạt [${maVP}]?`)) {
                row.remove();
                alert('Xóa phiếu phạt thành công!');
            }
        }
    });

    // Sự kiện đóng modal
    if (closeViolationModalBtn) closeViolationModalBtn.addEventListener('click', closeViolationModal);
    if (violationModal) violationModal.addEventListener('click', e => (e.target === violationModal) && closeViolationModal());

    // Sự kiện submit form
    if (violationForm) violationForm.addEventListener('submit', function (e) {
        e.preventDefault();
        alert('Lưu thông tin phiếu phạt thành công!');
        closeViolationModal();
    });
});
// --- LOGIC FOR CAI-DAT-HE-THONG.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    // Chỉ chạy code này nếu đang ở trang Cài đặt
    const settingsForm = document.getElementById('settings-form');
    if (settingsForm) {
        settingsForm.addEventListener('submit', function (event) {
            // Ngăn form gửi đi và tải lại trang
            event.preventDefault();

            // Hiển thị thông báo thành công (mô phỏng)
            alert('Cài đặt đã được lưu thành công!');
        });
    }
});

// --- LOGIC FOR THONG-TIN-CA-NHAN.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    // Chỉ chạy code này nếu đang ở trang Thông tin cá nhân
    const profileForm = document.getElementById('profile-form');
    if (profileForm) {
        profileForm.addEventListener('submit', function (event) {
            // Ngăn form gửi đi và tải lại trang
            event.preventDefault();

            const newPassword = document.getElementById('profile-new-password').value;
            const confirmPassword = document.getElementById('profile-confirm-password').value;

            // Kiểm tra mật khẩu có khớp không
            if (newPassword && newPassword !== confirmPassword) {
                alert('Mật khẩu mới và mật khẩu xác nhận không khớp!');
                return; // Dừng lại không thực hiện tiếp
            }

            // Hiển thị thông báo thành công (mô phỏng)
            alert('Cập nhật thông tin cá nhân thành công!');

            // Xóa các trường mật khẩu sau khi cập nhật thành công
            document.getElementById('profile-old-password').value = '';
            document.getElementById('profile-new-password').value = '';
            document.getElementById('profile-confirm-password').value = '';
        });
    }
});

// --- LOGIC FOR DANG-XUAT.HTML ---

document.addEventListener('DOMContentLoaded', function () {
    // Tìm container chính chứa thông tin đăng xuất
    const logoutContainer = document.getElementById('logout-container');

    // Chỉ chạy code nếu tìm thấy container này trên trang
    if (logoutContainer) {
        // Lấy URL chuyển hướng từ thuộc tính 'data-redirect-url'
        const redirectUrl = logoutContainer.dataset.redirectUrl;

        // Tìm phần tử hiển thị số giây đếm ngược
        const countdownElement = document.getElementById('countdown');

        // Lấy số giây ban đầu và chuyển thành số nguyên
        let seconds = parseInt(countdownElement.textContent, 10);

        // Bắt đầu bộ đếm thời gian, lặp lại mỗi giây
        const interval = setInterval(function () {
            seconds--; // Giảm số giây
            countdownElement.textContent = seconds; // Cập nhật lại số trên màn hình

            // Nếu đếm về 0
            if (seconds <= 0) {
                clearInterval(interval); // Dừng bộ đếm
                window.location.href = redirectUrl; // Chuyển hướng trang
            }
        }, 1000); // 1000ms = 1 giây
    }
});
// --- LOGIC FOR ADMIN PROFILE DROPDOWN MENU ---

document.addEventListener('DOMContentLoaded', function () {
    // Tìm container của dropdown
    const profileDropdown = document.querySelector('.profile-dropdown');

    // Chỉ chạy code nếu tìm thấy dropdown trên trang
    if (profileDropdown) {
        const button = profileDropdown.querySelector('.login-button');
        const dropdownContent = profileDropdown.querySelector('.dropdown-content');

        // Gắn sự kiện click cho nút bấm
        button.addEventListener('click', function (event) {
            // Ngăn sự kiện click này lan ra ngoài cửa sổ (window)
            event.stopPropagation();
            // Bật/tắt class 'show' để hiển thị hoặc ẩn menu
            dropdownContent.classList.toggle('show');
        });

        // Gắn sự kiện click cho toàn bộ cửa sổ để đóng dropdown khi nhấn ra ngoài
        window.addEventListener('click', function () {
            if (dropdownContent.classList.contains('show')) {
                dropdownContent.classList.remove('show');
            }
        });
    }
});

/* --- LOGIC FOR REAL-TIME CLOCK --- */

// Đảm bảo code chỉ chạy khi toàn bộ trang đã được tải
document.addEventListener('DOMContentLoaded', function () {
    // Tìm phần tử đồng hồ bằng ID chúng ta đã đặt ở Bước 1
    const clockElement = document.getElementById('real-time-clock');

    // Nếu không tìm thấy phần tử đồng hồ trên trang, không làm gì cả
    if (!clockElement) return;

    // Hàm chính để cập nhật đồng hồ
    function updateClock() {
        // Mảng chứa tên các thứ trong tuần bằng tiếng Việt
        const daysOfWeek = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];

        // Lấy thời gian hiện tại
        const now = new Date();

        // Lấy các thành phần của ngày và giờ
        const dayName = daysOfWeek[now.getDay()]; // Lấy tên thứ
        const day = String(now.getDate()).padStart(2, '0'); // Lấy ngày và thêm số 0 nếu cần
        const month = String(now.getMonth() + 1).padStart(2, '0'); // Tháng bắt đầu từ 0 nên phải +1
        const year = now.getFullYear();
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');

        // Tạo chuỗi thời gian hoàn chỉnh theo đúng định dạng bạn muốn
        // (Lưu ý: GMT+7 là cố định cho múi giờ Việt Nam)
        const formattedTime = `${dayName}, ${day}/${month}/${year}, ${hours}:${minutes} GMT+7`;

        // Cập nhật nội dung của phần tử đồng hồ trên trang web
        clockElement.textContent = formattedTime;
    }

    // Chạy hàm updateClock() ngay lập tức khi trang vừa tải xong để hiển thị thời gian đúng ngay
    updateClock();

    // Sau đó, cứ mỗi 1 giây (1000 mili giây) thì gọi lại hàm updateClock() để cập nhật
    setInterval(updateClock, 1000);
});

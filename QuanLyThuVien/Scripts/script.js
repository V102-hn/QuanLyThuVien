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


// --- LOGIC FOR QUAN-LY-DOC-GIA.HTML (PHIÊN BẢN HOÀN CHỈNH) ---
$(document).ready(function () {
    const readerModal = $('#reader-modal');
    if (readerModal.length === 0) return; // Chỉ chạy code nếu có modal

    // --- Lấy các phần tử ---
    const addReaderBtn = $('#add-reader-btn');
    const closeReaderModalBtn = $('.close-reader-modal');
    const readerModalTitle = $('#reader-modal-title');
    const readerForm = $('#reader-form');
    const readersTable = $('#readers-table');

    // --- Các hàm hỗ trợ ---
    function openReaderModal() { readerModal.addClass('show-modal'); }
    function closeReaderModal() { readerModal.removeClass('show-modal'); }

    // --- Gắn sự kiện ---

    // 1. Mở modal để THÊM độc giả
    addReaderBtn.on('click', function () {
        readerModalTitle.text("Thêm Độc giả Mới");
        readerForm[0].reset();
        $('#ma-doc-gia').val(''); // Xóa ID ẩn
        $('#username-modal').prop('readonly', false); // Cho phép nhập username
        openReaderModal();
    });

    // 2. Mở modal để SỬA độc giả
    readersTable.on('click', '.btn-edit', function () {
        const readerId = $(this).data('id');

        $.ajax({
            url: '/Readers/GetReaderDetails',
            type: 'GET',
            data: { id: readerId },
            success: function (response) {
                if (response.success) {
                    const data = response.data;
                    readerModalTitle.text("Chỉnh sửa Thông tin Độc giả");
                    readerForm[0].reset();

                    // Điền dữ liệu vào form
                    $('#ma-doc-gia').val(data.MaDocGia);
                    $('#ho-ten').val(data.HoTen);
                    $('#lop').val(data.Lop);
                    $('#email').val(data.Email);
                    $('#username-modal').val(data.Username).prop('readonly', true); // không cho sửa username
                    $('#tinh-trang-the').val(data.DaXoa.toString());

                    openReaderModal();
                } else {
                    alert('Lỗi: Không thể lấy thông tin độc giả.');
                }
            },
            error: function () {
                alert('Không thể kết nối đến server.');
            }
        });
    });

    // 3. Xử lý nút XÓA độc giả
    readersTable.on('click', '.btn-delete', function () {
        const button = $(this);
        const readerId = button.data('id');
        const readerName = button.closest('tr').find('td:eq(1)').text();

        if (confirm(`Bạn có chắc chắn muốn XÓA VĨNH VIỄN độc giả "${readerName}" không? Hành động này không thể hoàn tác.`)) {
            $.ajax({
                url: '/Readers/Delete',
                type: 'POST',
                data: { id: readerId },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);

                        // === THAY ĐỔI Ở ĐÂY ===
                        // Xóa dòng khỏi bảng trên giao diện với hiệu ứng mờ dần
                        button.closest('tr').fadeOut(500, function () {
                            $(this).remove();
                        });

                    } else {
                        alert('Lỗi: ' + response.message);
                    }
                },
                error: function () {
                    alert('Không thể kết nối đến server.');
                }
            });
        }
    });

    // 4. Xử lý SUBMIT FORM (cho cả Thêm và Sửa)
    readerForm.on('submit', function (event) {
        event.preventDefault();

        const readerId = $('#ma-doc-gia').val();
        const url = readerId ? '/Readers/Edit' : '/Readers/Create'; // Quyết định URL
        const formData = $(this).serialize();

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    closeReaderModal();
                    location.reload(); // Tải lại trang để cập nhật danh sách
                } else {
                    alert('Lỗi: ' + response.message);
                }
            },
            error: function () {
                alert('Không thể kết nối đến server.');
            }
        });
    });

    // 5. Đóng modal
    closeReaderModalBtn.on('click', closeReaderModal);
    readerModal.on('click', function (event) {
        if (event.target === readerModal[0]) {
            closeReaderModal();
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
// --- LOGIC FOR QUAN-LY-NGUOI-DUNG.HTML (PHIÊN BẢN HOÀN CHỈNH) ---
$(document).ready(function () {
    const userModal = $('#user-modal');
    if (userModal.length === 0) return; // Chỉ chạy nếu đang ở trang Quản lý Người dùng

    // --- Lấy các phần tử ---
    const addUserBtn = $('#add-user-btn');
    const closeUserModalBtn = $('.close-user-modal');
    const userModalTitle = $('#user-modal-title');
    const userForm = $('#user-form');
    const usersTable = $('#users-table');
    const searchInput = $('#user-search-input');

    // --- Các hàm hỗ trợ ---
    function openUserModal() { userModal.addClass('show-modal'); }
    function closeUserModal() { userModal.removeClass('show-modal'); }

    // --- Gắn sự kiện ---

    // 1. Mở modal để THÊM người dùng
    addUserBtn.on('click', function () {
        userModalTitle.text("Thêm Người dùng Mới");
        userForm[0].reset();
        $('#ma-thu-thu').val('');
        $('#username').prop('readonly', false);
        openUserModal();
    });

    // 2. Mở modal để SỬA người dùng
    usersTable.on('click', '.btn-edit', function () {
        const userId = $(this).data('id');

        $.ajax({
            url: '/NguoiDung/GetUserDetails',
            type: 'GET',
            data: { id: userId },
            success: function (response) {
                if (response.success) {
                    const data = response.data;
                    userModalTitle.text("Chỉnh sửa Người dùng");
                    userForm[0].reset();

                    // Điền dữ liệu vào form
                    $('#ma-thu-thu').val(data.MaThuThu);
                    $('#username').val(data.Username).prop('readonly', true); // Không cho sửa username
                    $('#full-name').val(data.HoTen);
                    $('#email-user').val(data.Email);
                    $('#role-select').val(data.IsAdmin.toString());
                    $('#status-select').val(data.DaXoa.toString());

                    openUserModal();
                } else {
                    alert('Lỗi: Không thể lấy thông tin người dùng.');
                }
            },
            error: function () {
                alert('Không thể kết nối đến server.');
            }
        });
    });

    // 3. Xử lý nút XÓA người dùng
    usersTable.on('click', '.btn-delete', function () {
        const button = $(this);
        const userId = button.data('id');
        const userName = button.closest('tr').find('td:eq(0)').text();

        if (confirm(`Bạn có chắc chắn muốn xóa người dùng "${userName}" không?`)) {
            $.ajax({
                url: '/NguoiDung/Delete',
                type: 'POST',
                data: { id: userId },
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
                    alert('Không thể kết nối đến server.');
                }
            });
        }
    });

    // 4. Xử lý SUBMIT FORM (cho cả Thêm và Sửa)
    userForm.on('submit', function (event) {
        event.preventDefault();

        const userId = $('#ma-thu-thu').val();
        const url = userId ? '/NguoiDung/Edit' : '/NguoiDung/Create';
        const formData = $(this).serialize();

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    closeUserModal();
                    location.reload();
                } else {
                    alert('Lỗi: ' + response.message);
                }
            },
            error: function () {
                alert('Không thể kết nối đến server.');
            }
        });
    });

    // 5. Chức năng TÌM KIẾM
    searchInput.on('keyup', function () {
        const value = $(this).val().toLowerCase();
        $("#users-table tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // 6. Đóng modal
    closeUserModalBtn.on('click', closeUserModal);
    userModal.on('click', function (event) {
        if (event.target === userModal[0]) {
            closeUserModal();
        }
    });
});

// --- LOGIC FOR QUAN-LY-MUON-TRA.HTML (PHIÊN BẢN HOÀN CHỈNH VỚI TẤT CẢ CHỨC NĂNG) ---
$(document).ready(function () {
    const loanModal = $('#loan-modal');
    if (loanModal.length === 0) return; // Chỉ chạy code nếu đang ở trang Mượn Trả

    // --- Lấy các phần tử DOM ---
    const addLoanBtn = $('#add-loan-btn');
    const closeLoanModalBtns = $('.close-loan-modal, .close-loan-modal-btn');
    const loanModalTitle = $('#loan-modal-title');
    const loanForm = $('#loan-form');
    const addBookToListBtn = $('#add-book-to-list-btn');
    const sachSelect = $('#sach-select');
    const selectedBooksList = $('#selected-books-list');
    const dataTableBody = $('.data-table-section tbody');

    // --- Các hàm hỗ trợ ---
    function openLoanModal() { loanModal.addClass('show-modal'); }
    function closeLoanModal() { loanModal.removeClass('show-modal'); }

    function setDefaultDates() {
        const today = new Date();
        const twoWeeksLater = new Date();
        twoWeeksLater.setDate(today.getDate() + 14);
        const formatDate = (date) => date.toISOString().split('T')[0];
        $('#ngay-muon').val(formatDate(today));
        $('#ngay-hen-tra').val(formatDate(twoWeeksLater));
    }

    // Hàm chung để lấy dữ liệu chi tiết và điền vào modal
    function populateModalWithDetails(id, isViewOnly) {
        $.ajax({
            url: '/MuonTra/GetMuonTraDetails',
            type: 'GET',
            data: { id: id },
            success: function (response) {
                if (!response.success) {
                    alert('Lỗi: ' + response.message);
                    return;
                }
                const data = response.data;
                loanForm[0].reset();
                selectedBooksList.empty();

                // Điền thông tin chung của phiếu mượn
                $('#ma-muon-tra-id').val(data.phieuMuon.MaMuonTra);
                $('#ma-phieu-display').val(data.phieuMuon.MaMuonTra);
                $('#doc-gia-select').val(data.phieuMuon.MaDocGia);
                // Chuyển đổi định dạng ngày tháng từ JSON của .NET
                $('#ngay-muon').val(new Date(parseInt(data.phieuMuon.NgayMuon.substr(6))).toISOString().split('T')[0]);
                $('#ngay-hen-tra').val(new Date(parseInt(data.phieuMuon.NgayHenTra.substr(6))).toISOString().split('T')[0]);
                $('#ghi-chu').val(data.ghiChu);

                // Điền danh sách các sách đã mượn
                data.chiTietSach.forEach(sach => {
                    // Chế độ Xem/Sửa không cần nút xóa sách
                    const listItem = `<li><span>${sach.TenSachDisplay}</span></li>`;
                    selectedBooksList.append(listItem);
                });

                // Tùy chỉnh modal dựa trên chế độ Xem hoặc Sửa
                if (isViewOnly) {
                    loanModalTitle.text("Chi tiết Phiếu Mượn #" + id);
                    // Khóa tất cả các trường
                    loanForm.find('input, select, textarea, button').prop('disabled', true);
                    // Luôn cho phép đóng modal
                    $('.close-loan-modal, .close-loan-modal-btn').prop('disabled', false);
                } else { // Chế độ Sửa
                    loanModalTitle.text("Chỉnh sửa Phiếu Mượn #" + id);
                    // Mở khóa tất cả các trường trước
                    loanForm.find('input, select, textarea, button').prop('disabled', false);
                    // Sau đó khóa các trường không được phép sửa
                    $('#ma-phieu-display, #doc-gia-select, #ngay-muon, #sach-select, #add-book-to-list-btn').prop('disabled', true);
                }
                openLoanModal();
            },
            error: function () {
                alert('Không thể kết nối đến server để lấy dữ liệu.');
            }
        });
    }

    // --- Gắn các sự kiện cho các nút ---

    // 1. Sự kiện nhấn nút "Tạo Phiếu Mượn"
    addLoanBtn.on('click', function () {
        loanModalTitle.text("Tạo Phiếu Mượn Mới");
        loanForm[0].reset();
        selectedBooksList.empty();
        $('#ma-muon-tra-id').val(''); // Quan trọng: Xóa ID để form biết đây là chế độ tạo mới
        $('#ma-phieu-display').val('');
        loanForm.find('input, select, textarea, button').prop('disabled', false); // Mở khóa tất cả các trường
        setDefaultDates();
        openLoanModal();
    });

    // 2. Sự kiện nhấn nút "Chi tiết"
    dataTableBody.on('click', '.btn-view-details', function () {
        populateModalWithDetails($(this).data('id'), true); // true = chế độ chỉ xem
    });

    // 3. Sự kiện nhấn nút "Sửa"
    dataTableBody.on('click', '.btn-edit', function () {
        populateModalWithDetails($(this).data('id'), false); // false = chế độ sửa
    });

    // 4. Sự kiện nhấn nút "Trả sách"
    dataTableBody.on('click', '.btn-mark-returned', function () {
        const button = $(this);
        const maPhieu = button.data('id');
        if (confirm(`Bạn có chắc chắn muốn xác nhận trả sách cho phiếu [${maPhieu}] không?`)) {
            $.ajax({
                url: '/MuonTra/MarkAsReturned',
                type: 'POST', data: { id: maPhieu },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        location.reload();
                    } else { alert('Lỗi: ' + response.message); }
                },
                error: function () { alert('Không thể kết nối đến server.'); }
            });
        }
    });

    // 5. Sự kiện nhấn nút "Xóa"
    dataTableBody.on('click', '.btn-delete', function () {
        const button = $(this);
        const maPhieu = button.data('id');
        if (confirm(`Bạn có chắc chắn muốn XÓA phiếu mượn [${maPhieu}] không?`)) {
            $.ajax({
                url: '/MuonTra/Delete',
                type: 'POST', data: { id: maPhieu },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        button.closest('tr').fadeOut(500, function () { $(this).remove(); });
                    } else { alert('Lỗi: ' + response.message); }
                },
                error: function () { alert('Không thể kết nối đến server.'); }
            });
        }
    });

    // --- Logic bên trong Modal ---

    // Sự kiện nhấn nút "Thêm sách" vào danh sách
    addBookToListBtn.on('click', function () {
        const selectedOption = sachSelect.find('option:selected');
        const bookId = selectedOption.val();
        const bookText = selectedOption.text();
        if (bookId) {
            if (selectedBooksList.find(`li[data-book-id="${bookId}"]`).length > 0) {
                alert('Sách này đã được thêm vào danh sách!'); return;
            }
            const listItem = `<li data-book-id="${bookId}"><span>${bookText}</span><button type="button" class="remove-book-btn">&times;</button></li>`;
            selectedBooksList.append(listItem);
        }
    });

    // Sự kiện nhấn nút "x" để xóa sách khỏi danh sách
    selectedBooksList.on('click', '.remove-book-btn', function () {
        $(this).closest('li').remove();
    });

    // Sự kiện SUBMIT FORM (xử lý cả TẠO MỚI và SỬA)
    loanForm.on('submit', function (event) {
        event.preventDefault();
        const maMuonTra = $('#ma-muon-tra-id').val();

        let url;
        let dataToSend;
        let contentType = 'application/x-www-form-urlencoded; charset=UTF-8'; // Mặc định cho form data

        if (maMuonTra) { // --- CHẾ ĐỘ SỬA ---
            url = '/MuonTra/Edit';
            dataToSend = {
                MaMuonTra: maMuonTra,
                NgayHenTra: $('#ngay-hen-tra').val(),
                GhiChu: $('#ghi-chu').val()
            };
            // Dữ liệu sửa là JSON
            contentType = 'application/json; charset=utf-8';
            dataToSend = JSON.stringify(dataToSend);

        } else { // --- CHẾ ĐỘ TẠO MỚI ---
            url = '/MuonTra/Create';
            const sachIds = [];
            selectedBooksList.find('li').each(function () { sachIds.push($(this).data('book-id')); });

            if (sachIds.length === 0) { alert('Vui lòng chọn ít nhất một cuốn sách!'); return; }

            dataToSend = {
                MaDocGia: $('#doc-gia-select').val(),
                NgayMuon: $('#ngay-muon').val(),
                NgayHenTra: $('#ngay-hen-tra').val(),
                GhiChu: $('#ghi-chu').val(),
                SachIds: sachIds
            };
            // Dữ liệu tạo mới là JSON
            contentType = 'application/json; charset=utf-8';
            dataToSend = JSON.stringify(dataToSend);
        }

        $.ajax({
            url: url,
            type: 'POST',
            contentType: contentType,
            data: dataToSend,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    closeLoanModal();
                    location.reload();
                } else {
                    alert('Lỗi: ' + response.message);
                }
            },
            error: function () { alert('Không thể kết nối đến server.'); }
        });
    });

    // Sự kiện đóng modal
    closeLoanModalBtns.on('click', closeLoanModal);
    loanModal.on('click', function (event) {
        if (event.target === loanModal[0]) {
            closeLoanModal();
        }
    });
});

// --- LOGIC FOR QUAN-LY-NXB.HTML (PHIÊN BẢN HOÀN CHỈNH) ---
$(document).ready(function () {
    const nxbModal = $('#nxb-modal');
    if (nxbModal.length === 0) return; // Chỉ chạy code này nếu đang ở trang NXB

    // --- Lấy các phần tử ---
    const addNxbBtn = $('#add-nxb-btn');
    const closeNxbModalBtn = $('.close-nxb-modal');
    const nxbModalTitle = $('#nxb-modal-title');
    const nxbForm = $('#nxb-form');
    const nxbTable = $('#nxb-table');
    const searchInput = $('#nxb-search-input');

    // --- Các hàm hỗ trợ ---
    function openNxbModal() { nxbModal.addClass('show-modal'); }
    function closeNxbModal() { nxbModal.removeClass('show-modal'); }

    // --- Gắn sự kiện ---

    // 1. Mở modal để THÊM NXB
    addNxbBtn.on('click', function () {
        nxbModalTitle.text("Thêm Nhà Xuất Bản Mới");
        nxbForm[0].reset();
        $('#ma-nxb-hidden').val(''); // Xóa ID ẩn
        openNxbModal();
    });

    // 2. Mở modal để SỬA NXB
    nxbTable.on('click', '.btn-edit', function () {
        const nxbId = $(this).data('id');

        $.ajax({
            url: '/NXB/GetNXBDetails',
            type: 'GET',
            data: { id: nxbId },
            success: function (response) {
                if (response.success) {
                    const data = response.data;
                    nxbModalTitle.text("Chỉnh sửa Nhà Xuất Bản");
                    nxbForm[0].reset();

                    // Điền dữ liệu vào form
                    $('#ma-nxb-hidden').val(data.MaNXB);
                    $('#ten-nxb').val(data.TenNXB);
                    $('#dia-chi').val(data.DiaChi);
                    $('#sdt-nxb').val(data.SoDienThoai);
                    $('#email-nxb').val(data.Email);

                    openNxbModal();
                } else {
                    alert('Lỗi: Không thể lấy thông tin NXB.');
                }
            },
            error: function () {
                alert('Không thể kết nối đến server.');
            }
        });
    });

    // 3. Xử lý nút XÓA NXB
    nxbTable.on('click', '.btn-delete', function () {
        const button = $(this);
        const nxbId = button.data('id');
        const nxbName = button.closest('tr').find('td:eq(1)').text();

        if (confirm(`Bạn có chắc chắn muốn xóa nhà xuất bản "${nxbName}" không?`)) {
            $.ajax({
                url: '/NXB/Delete',
                type: 'POST',
                data: { id: nxbId },
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
                    alert('Không thể kết nối đến server.');
                }
            });
        }
    });

    // 4. Xử lý SUBMIT FORM (cho cả Thêm và Sửa)
    nxbForm.on('submit', function (event) {
        event.preventDefault();

        const nxbId = $('#ma-nxb-hidden').val();
        const url = nxbId ? '/NXB/Edit' : '/NXB/Create'; // Quyết định URL dựa trên việc có ID hay không
        const formData = $(this).serialize();

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    closeNxbModal();
                    location.reload(); // Tải lại trang để cập nhật danh sách
                } else {
                    alert('Lỗi: ' + response.message);
                }
            },
            error: function () {
                alert('Không thể kết nối đến server.');
            }
        });
    });

    // 5. Chức năng TÌM KIẾM (Client-side)
    searchInput.on('keyup', function () {
        const value = $(this).val().toLowerCase();
        $("#nxb-table tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // 6. Đóng modal
    closeNxbModalBtn.on('click', closeNxbModal);
    nxbModal.on('click', function (event) {
        if (event.target === nxbModal[0]) {
            closeNxbModal();
        }
    });
});

// --- LOGIC FOR QUAN-LY-TAC-GIA.HTML (PHIÊN BẢN HOÀN CHỈNH) ---
$(document).ready(function () {
    const authorModal = $('#author-modal');
    if (authorModal.length === 0) return; // Chỉ chạy code này nếu đang ở trang Tác giả

    // --- Lấy các phần tử ---
    const addAuthorBtn = $('#add-author-btn');
    const closeAuthorModalBtn = $('.close-author-modal');
    const authorModalTitle = $('#author-modal-title');
    const authorForm = $('#author-form');
    const authorTable = $('#author-table');
    const searchInput = $('#author-search-input');

    // --- Các hàm hỗ trợ ---
    function openAuthorModal() { authorModal.addClass('show-modal'); }
    function closeAuthorModal() { authorModal.removeClass('show-modal'); }

    // --- Gắn sự kiện ---

    // 1. Mở modal để THÊM tác giả
    addAuthorBtn.on('click', function () {
        authorModalTitle.text("Thêm Tác giả Mới");
        authorForm[0].reset();
        $('#ma-tac-gia').val(''); // Xóa ID ẩn
        openAuthorModal();
    });

    // 2. Mở modal để SỬA tác giả
    authorTable.on('click', '.btn-edit', function () {
        const authorId = $(this).data('id');

        $.ajax({
            url: '/TacGia/GetTacGiaDetails',
            type: 'GET',
            data: { id: authorId },
            success: function (response) {
                if (response.success) {
                    const data = response.data;
                    authorModalTitle.text("Chỉnh sửa Tác giả");
                    authorForm[0].reset();

                    // Điền dữ liệu vào form
                    $('#ma-tac-gia').val(data.MaTacGia);
                    $('#ten-tac-gia').val(data.TenTacGia);
                    $('#tieu-su').val(data.TieuSu);

                    openAuthorModal();
                } else {
                    alert('Lỗi: Không thể lấy thông tin tác giả.');
                }
            },
            error: function () {
                alert('Không thể kết nối đến server.');
            }
        });
    });

    // 3. Xử lý nút XÓA tác giả
    authorTable.on('click', '.btn-delete', function () {
        const button = $(this);
        const authorId = button.data('id');
        const authorName = button.closest('tr').find('td:eq(1)').text();

        if (confirm(`Bạn có chắc chắn muốn xóa tác giả "${authorName}" không?`)) {
            $.ajax({
                url: '/TacGia/Delete',
                type: 'POST',
                data: { id: authorId },
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
                    alert('Không thể kết nối đến server.');
                }
            });
        }
    });

    // 4. Xử lý SUBMIT FORM (cho cả Thêm và Sửa)
    authorForm.on('submit', function (event) {
        event.preventDefault();

        const authorId = $('#ma-tac-gia').val();
        const url = authorId ? '/TacGia/Edit' : '/TacGia/Create';
        const formData = $(this).serialize();

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    closeAuthorModal();
                    location.reload();
                } else {
                    alert('Lỗi: ' + response.message);
                }
            },
            error: function () {
                alert('Không thể kết nối đến server.');
            }
        });
    });

    // 5. Chức năng TÌM KIẾM (Client-side)
    searchInput.on('keyup', function () {
        const value = $(this).val().toLowerCase();
        $("#author-table tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // 6. Đóng modal
    closeAuthorModalBtn.on('click', closeAuthorModal);
    authorModal.on('click', function (event) {
        if (event.target === authorModal[0]) {
            closeAuthorModal();
        }
    });
});
// --- LOGIC FOR QUAN-LY-VI-PHAM.HTML (PHIÊN BẢN CUỐI CÙNG VỚI CHỨC NĂNG SỬA) ---
$(document).ready(function () {
    const violationModal = $('#violation-modal');
    if (violationModal.length === 0) return;

    const addViolationBtn = $('#add-violation-btn');
    const closeViolationModalBtn = $('.close-violation-modal');
    const violationModalTitle = $('#violation-modal-title');
    const violationForm = $('#violation-form');
    const violationTable = $('#violation-table');
    const searchInput = $('#violation-search-input');
    const filterStatus = $('#violation-filter-status');

    function openViolationModal() { violationModal.addClass('show-modal'); }
    function closeViolationModal() { violationModal.removeClass('show-modal'); }

    // Mở modal để LẬP PHIẾU PHẠT MỚI
    addViolationBtn.on('click', function () {
        violationModalTitle.text("Lập Phiếu Phạt Mới");
        violationForm[0].reset();
        violationForm.find('input[name="MaViPham"]').val('');
        openViolationModal();
    });

    // Mở modal để SỬA PHIẾU PHẠT
    violationTable.on('click', '.btn-edit', function () {
        const violationId = $(this).data('id');
        $.ajax({
            url: '/XuLyViPham/GetViPhamDetails', type: 'GET', data: { id: violationId },
            success: function (response) {
                if (response.success) {
                    const data = response.data;
                    violationModalTitle.text("Chỉnh sửa Phiếu Phạt #VP" + data.MaViPham.toString().padStart(3, '0'));
                    violationForm[0].reset();
                    violationForm.find('input[name="MaViPham"]').val(data.MaViPham);
                    $('#doc-gia-vp').val(data.MaDocGia);
                    $('#loai-vi-pham').val(data.LoaiViPham);
                    $('#so-tien-phat').val(data.HinhThucPhat);
                    $('#ghi-chu-vp').val(data.GhiChu);
                    openViolationModal();
                } else { alert('Lỗi: ' + response.message); }
            },
            error: function () { alert('Không thể kết nối đến server.'); }
        });
    });

    // Xử lý nút "Đã trả"
    violationTable.on('click', '.btn-mark-paid', function () {
        const button = $(this);
        const violationId = button.data('id');
        if (confirm(`Xác nhận độc giả đã thanh toán khoản phạt cho phiếu [VP${violationId.toString().padStart(3, '0')}]?`)) {
            $.ajax({
                url: '/XuLyViPham/MarkAsPaid', type: 'POST', data: { id: violationId },
                success: function (response) {
                    if (response.success) { alert(response.message); location.reload(); }
                    else { alert('Lỗi: ' + response.message); }
                },
                error: function () { alert('Không thể kết nối đến server.'); }
            });
        }
    });

    // Xử lý nút XÓA
    violationTable.on('click', '.btn-delete', function () {
        const button = $(this);
        const violationId = button.data('id');
        if (confirm(`Bạn có chắc chắn muốn xóa phiếu phạt [VP${violationId.toString().padStart(3, '0')}] không?`)) {
            $.ajax({
                url: '/XuLyViPham/Delete', type: 'POST', data: { id: violationId },
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        button.closest('tr').fadeOut(500, function () { $(this).remove(); });
                    } else { alert('Lỗi: ' + response.message); }
                },
                error: function () { alert('Không thể kết nối đến server.'); }
            });
        }
    });

    // Xử lý SUBMIT FORM (cho cả Thêm và Sửa)
    violationForm.on('submit', function (event) {
        event.preventDefault();
        const violationId = $(this).find('input[name="MaViPham"]').val();
        const url = violationId ? '/XuLyViPham/Edit' : '/XuLyViPham/Create';
        const formData = $(this).serialize();
        $.ajax({
            url: url, type: 'POST', data: formData,
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    closeViolationModal();
                    location.reload();
                } else { alert('Lỗi: ' + response.message); }
            },
            error: function () { alert('Không thể kết nối đến server.'); }
        });
    });

    // LỌC và TÌM KIẾM
    function filterTable() {
        const searchText = searchInput.val().toLowerCase();
        const statusFilter = filterStatus.val();
        $("#violation-table tbody tr").each(function () {
            const row = $(this);
            const rowText = row.text().toLowerCase();
            const rowStatus = row.find('td:eq(5)').text().trim();
            const textMatch = rowText.indexOf(searchText) > -1;
            const statusMatch = (statusFilter === 'all' || statusFilter === '' || rowStatus === statusFilter);
            if (textMatch && statusMatch) { row.show(); } else { row.hide(); }
        });
    }
    searchInput.on('keyup', filterTable);
    filterStatus.on('change', filterTable);

    // Đóng modal
    closeViolationModalBtn.on('click', closeViolationModal);
    violationModal.on('click', function (event) {
        if (event.target === violationModal[0]) { closeViolationModal(); }
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

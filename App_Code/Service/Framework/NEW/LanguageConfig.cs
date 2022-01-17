using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Config for lanuage
/// @CongTT
/// </summary>
public class LanguageConfig
{
    public static string INTRA_HEADER_VI_VALUE = "Chuyển khoản trong SHB";
    public static string INTRA_HEADER_EN_VALUE = "Intra transfer";

    public static string INTER_HEADER_VI_VALUE = "Chuyển khoản liên ngân hàng";
    public static string INTER_HEADER_EN_VALUE = "Inter transfer";

    public static string ACCT_NUMBER_HEADER_VI_VALUE = "Tài khoản nhận";

    public static string ACCT_NUMBER_HEADER_EN_VALUE = "To account";

    public static string ACCT_NAME_HEADER_VI_VALUE = "Tên tài khoản nhận";
    public static string ACCT_NAME_HEADER_EN_VALUE = "Account name";

    public static string ErrorVoucherVi99 = "Mã voucher không hợp lệ.\nBạn vẫn muốn tiếp tục thanh toán?";

    public static string ErrorVoucherEn99 = "Voucher code is invalid.\nDo you want to pay?";

    public static string ErrorVoucherVi62 = "Đơn hàng không được giảm giá do ngân hàng thanh toán không hỗ trợ.\nQuý khách có muốn tiếp tục giao dịch?";

    public static string ErrorVoucherEn62 = "Đơn hàng không được giảm giá do ngân hàng thanh toán không hỗ trợ.\nQuý khách có muốn tiếp tục giao dịch?";

    public static string ErrorVoucherVi01 = "Mã QRCode này đã được thanh toán.";
    public static string ErrorVoucherVi60_61 = "Mã giảm giá không đúng.\nQuý khách có muốn tiếp tục giao dịch?";
    public static string ErrorVoucherVi63 = "Đơn hàng không được giảm giá do đơn vị kinh doanh chưa tham gia chương trình.\nQuý khách có muốn tiếp tục giao dịch?";

    public static string ErrorVoucherVi64 = "Đơn hàng không được giảm giá do đã hết số lần áp dụng/KH.\nQuý khách có muốn tiếp tục giao dịch?";
    public static string ErrorVoucherVi65 = "Đơn hàng không được giảm giá do số tiền thanh toán nhỏ hơn số tiền được khuyến mại quy định.\nQuý khách có muốn tiếp tục giao dịch";
    public static string ErrorVoucherVi66 = "Số lượng mã giảm giá đã hết.\nQuý khách có muốn tiếp tục giao dịch?";
    public static string ErrorVoucherVi67 = "Mã giảm giá chưa được áp dụng.\nQuý khách có muốn tiếp tục giao dịch?";
    public static string ErrorVoucherVi68 = "Mã giảm giá đang bị khóa.\nQuý khách có muốn tiếp tục giao dịch?";
    public static string ErrorVoucherVi69 = "Mã giảm giá đã hết hiệu lực.\nQuý khách có muốn tiếp tục giao dịch?";


    public static string ErrorVirtualMoneyVi = "Theo yêu cầu của NHNN, SHB không hỗ trợ các giao dịch chuyển khoản liên quan đến tiền ảo";
    public static string ErrorVirtualMoneyEn = "At the request of the State Bank, SHB does not support the transfer of money related to virtual money";
}
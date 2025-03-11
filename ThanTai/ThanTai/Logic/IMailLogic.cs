using ThanTai.Models;
using WebBanHang.Models;

namespace WebBanHang.Logic
{
    public interface IMailLogic
    {
        Task GoiEmail(MailInfo mailInfo);
        Task GoiEmailDatHangThanhCong(DatHang datHang, MailInfo mailInfo);
    }
}

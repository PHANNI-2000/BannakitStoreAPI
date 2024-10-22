using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannakitStore.Utility
{
    public static class Constants
    {
        public static class OrderStatus
        {
            public const string PENDING = "Pending";
        }

        public static class PaymentStatus
        {
            public const int NO_PAYMENT = 1;
            public const int FULL_PAYMENT = 2;
            public const int ADVANCE_PAYMENT = 3;

            public static (string messageTh, string messageEn) GetMessage(int status)
            {
                string messageTh, messageEn;

                switch (status)
                {
                    case NO_PAYMENT:
                        messageTh = "ยังไม่มีการชำระเงิน";
                        messageEn = "No Payment";
                        break;
                    case FULL_PAYMENT:
                        messageTh = "ชำระเงินเต็มจำนวน";
                        messageEn = "Full Payment";
                        break;
                    case ADVANCE_PAYMENT:
                        messageTh = "ชำระเงินล่วงหน้า";
                        messageEn = "Advance Payment";
                        break;
                    default:
                        messageTh = "";
                        messageEn = "";
                        break;
                }
                // Tuple: เป็นโครงสร้างข้อมูลที่สามารถเก็บหลายค่าไว้ในหนึ่งออบเจ็กต์ โดยที่ค่าเหล่านั้นสามารถมีประเภทที่แตกต่างกันได้
                return (messageTh, messageEn); // return tuple
            }

        }
    }
}

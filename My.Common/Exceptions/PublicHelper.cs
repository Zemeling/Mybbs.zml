using My.Common.Exceptions.Extension;
using My.Common.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Exceptions
{
    public static class PublicHelper
    {
        public static void CheckArgument(object arg, string argName, bool canZero = false)
        {
            ArgumentNullException e3;
            if (arg == null)
            {
                e3 = new ArgumentNullException(argName);
                throw PublicHelper.ThrowComponentException(string.Format("参数 {0} 为空引发异常。", argName), e3);
            }
            Type type = arg.GetType();
            if (type.IsValueType && type.IsNumeric() && ((!canZero) ? (arg.CastTo(0.0) <= 0.0) : (arg.CastTo(0.0) < 0.0)))
            {
                ArgumentOutOfRangeException e2 = new ArgumentOutOfRangeException(argName);
                throw PublicHelper.ThrowComponentException(string.Format("参数 {0} 不在有效范围内引发异常。具体信息请查看系统日志。", argName), e2);
            }
            if (!(type == typeof(Guid)) || !((Guid)arg == Guid.Empty))
            {
                return;
            }
            e3 = new ArgumentNullException(argName);
            throw PublicHelper.ThrowComponentException(string.Format("参数{0}为空Guid引发异常。", argName), e3);
        }

        public static ComponentException ThrowComponentException(string msg, Exception e = null)
        {
            if (string.IsNullOrEmpty(msg) && e != null)
            {
                msg = e.Message;
            }
            else if (string.IsNullOrEmpty(msg))
            {
                msg = "未知组件异常，详情请查看日志信息。";
            }
            return (e == null) ? new ComponentException(string.Format("组件异常：{0}", msg)) : new ComponentException(string.Format("组件异常：{0}", msg), e);
        }

        public static DataAccessException ThrowDataAccessException(string msg, Exception e = null)
        {
            if (string.IsNullOrEmpty(msg) && e != null)
            {
                msg = e.Message;
            }
            else if (string.IsNullOrEmpty(msg))
            {
                msg = "未知数据访问层异常，详情请查看日志信息。";
            }
            return (e == null) ? new DataAccessException(string.Format("数据访问层异常：{0}", msg)) : new DataAccessException(string.Format("数据访问层异常：{0}", msg), e);
        }

        public static BusinessException ThrowBusinessException(string msg, Exception e = null)
        {
            if (string.IsNullOrEmpty(msg) && e != null)
            {
                msg = e.Message;
            }
            else if (string.IsNullOrEmpty(msg))
            {
                msg = "未知业务逻辑层异常，详情请查看日志信息。";
            }
            return (e == null) ? new BusinessException(string.Format("业务逻辑层异常：{0}", msg)) : new BusinessException(string.Format("业务逻辑层异常：{0}", msg), e);
        }
    }
}

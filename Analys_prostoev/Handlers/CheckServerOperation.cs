using System.Net.NetworkInformation;

namespace AnalysisDowntimes
{
	public static class CheckServerOperation
	{
		public static bool PingHost()
		{
			bool pingable = false;
			Ping pinger = null;

			try
			{
				pinger = new Ping();
				PingReply reply = pinger.Send("10.241.16.44");
				pingable = reply.Status == IPStatus.Success;
			}
			catch (PingException)
			{
				return false;
			}
			finally
			{
				if (pinger != null)
				{
					pinger.Dispose();
				}
			}

			return pingable;
		}
	}
}

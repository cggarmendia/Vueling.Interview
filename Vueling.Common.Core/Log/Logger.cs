using System;
using System.IO;

namespace Vueling.Common.Core.Log
{
    /// <summary>
    /// Clase que implementa un Logger
    /// </summary>
    public static class Logger
    {
        #region Miembros de la clase

        private static readonly object m_lockObj = new object();
        private static bool IsInitialized = false;
        private static LogLevel m_logLevel;
        private static string[] m_strLogFile = new string[5];
        private static TextWriter m_textWriter;

        #endregion Miembros de la clase

        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        static Logger()
        {
            m_logLevel = LogLevel.Error;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Agrega un Error al log
        /// </summary>
        /// <param name="p_strError">Texto del error</param>
        public static void AddLOGErr(string p_strError)
        {
            if (!IsInitialized)
                return;

            // logueo siempre, no importa el nivel seteado

            bool l_bNewLog = false;

            try
            {
                lock (m_lockObj)
                {
                    // Grabamos el mensaje
                    m_textWriter.WriteLine("<tr>");
                    m_textWriter.WriteLine("<td width='150'></td>");
                    m_textWriter.WriteLine("<td width='500'></td>");
                    m_textWriter.WriteLine("</tr>");

                    m_textWriter.WriteLine("<tr>");
                    m_textWriter.WriteLine("<td width='150'><font color='#FF0000'>" +
                                           String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now) + "</td>");
                    m_textWriter.WriteLine("<td width='500'><font color='#FF0000'>" + ReplaceEnters(p_strError) +
                                           "</font></td>");
                    m_textWriter.WriteLine("</tr>");

                    m_textWriter.WriteLine("<tr>");
                    m_textWriter.WriteLine("<td width='150'></td>");
                    m_textWriter.WriteLine("<td width='500'></td>");
                    m_textWriter.WriteLine("</tr>");
                    m_textWriter.Flush();
                }
                // Verificamos si el archivo supera los 5MB
                FileInfo info = new FileInfo(m_strLogFile[0]);
                l_bNewLog = (info.Length > 5242880);
            }
            catch
            {
                return;
            }

            // Si se excedio el tamaño establecido
            if (l_bNewLog)
            {
                lock (m_lockObj)
                {
                    // Pasamos a un nuevo archivo de LOG
                    MakeNewLOGFile();
                }
                AddLOGMsg("Inciado por LOG anterior excedido en tamaño (5MB)");
            }
        }

        /// <summary>
        /// Agrega un mensaje normal al log
        /// </summary>
        /// <param name="p_strMensaje">Texto del mensaje</param>
        public static void AddLOGMsg(string p_strMensaje)
        {
            if (!IsInitialized)
                return;

            // solo logueo si está habilitado nivel debug
            if (m_logLevel != LogLevel.Debug)
                return;

            bool l_bNewLog = false;

            try
            {
                lock (m_lockObj)
                {
                    // Grabamos el mensaje
                    m_textWriter.WriteLine("<tr>");

                    if (p_strMensaje.Trim() == "")
                    {
                        // El mensaje está vacío
                        m_textWriter.WriteLine("<td width='150'>&nbsp;</td>");
                        m_textWriter.WriteLine("<td width='500'>&nbsp;</td>");
                    }
                    else
                    {
                        // El mensaje tiene texto
                        m_textWriter.WriteLine("<td width='150'><font color='#000000'>" +
                                               String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now) + "</td>");
                        m_textWriter.WriteLine("<td width='500'><font color='#000000'>" + ReplaceEnters(p_strMensaje) +
                                               "</font></td>");
                    }

                    m_textWriter.WriteLine("</tr>");

                    m_textWriter.Flush();
                }

                // Verificamos si el archivo supera los 5MB
                FileInfo info = new FileInfo(m_strLogFile[0]);
                l_bNewLog = (info.Length > 5242880);
            }
            catch
            {
                return;
            }

            // Si se excedio el tamaño establecido
            if (l_bNewLog)
            {
                lock (m_lockObj)
                {
                    // Pasamos a un nuevo archivo de LOG
                    MakeNewLOGFile();
                }

                AddLOGMsg("Inciado por LOG anterior excedido en tamaño (5MB)");
            }
        }

        /// <summary>
        /// Agrega un mensaje destacado al log
        /// </summary>
        /// <param name="p_strMensaje">Texto del mensaje</param>
        public static void AddLOGSMsg(string p_strMensaje)
        {
            if (!IsInitialized)
                return;

            // solo logueo si está habilitado nivel Debug o Warning
            if (m_logLevel == LogLevel.Error)
                return;

            bool l_bNewLog = false;

            try
            {
                lock (m_lockObj)
                {
                    // Grabamos el mensaje
                    m_textWriter.WriteLine("<tr>");

                    if (p_strMensaje.Trim() == "")
                    {
                        // El mensaje está vacío
                        m_textWriter.WriteLine("<td width='150'>&nbsp;</td>");
                        m_textWriter.WriteLine("<td width='500'>&nbsp;</td>");
                    }
                    else
                    {
                        // El mensaje tiene texto
                        m_textWriter.WriteLine("<td width='150'><font color='#0000FF'>" +
                                               String.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now) + "</td>");
                        m_textWriter.WriteLine("<td width='500'><font color='#0000FF'>" + ReplaceEnters(p_strMensaje) +
                                               "</font></td>");
                    }

                    m_textWriter.WriteLine("</tr>");
                    m_textWriter.Flush();
                }
                // Verificamos si el archivo supera los 5MB
                FileInfo info = new FileInfo(m_strLogFile[0]);
                l_bNewLog = (info.Length > 5242880);
            }
            catch
            {
                return;
            }

            // Si se excedio el tamaño establecido
            if (l_bNewLog)
            {
                lock (m_lockObj)
                {
                    // Pasamos a un nuevo archivo de LOG
                    MakeNewLOGFile();
                }
                AddLOGMsg("Inciado por LOG anterior excedido en tamaño (5MB)");
            }
        }

        /// <summary>
        /// Inicializa el Log
        /// </summary>
        /// <param name="p_strPath">Path para los archivos</param>
        /// <param name="p_logLevel">Nivel de log deseado</param>
        /// <returns>T:Creado/F:Error</returns>
        public static bool Init(string p_strPath, string p_strAppName, LogLevel p_logLevel)
        {
            m_logLevel = p_logLevel;

            try
            {
                lock (m_lockObj)
                {
                    // Generamos los nombres de los archivos de Log
                    m_strLogFile[0] = p_strPath + string.Format("\\{0}.Log.html", p_strAppName);
                    m_strLogFile[1] = p_strPath + string.Format("\\{0}.Log.1.html", p_strAppName);
                    m_strLogFile[2] = p_strPath + string.Format("\\{0}.Log.2.html", p_strAppName);
                    m_strLogFile[3] = p_strPath + string.Format("\\{0}.Log.3.html", p_strAppName);
                    m_strLogFile[4] = p_strPath + string.Format("\\{0}.Log.4.html", p_strAppName);

                    // Creamos el nuevo archivo de log sin no hay uno
                    if (!File.Exists(m_strLogFile[0]))
                        MakeNewLOGFile();
                    else
                        m_textWriter = TextWriter.Synchronized(File.AppendText(m_strLogFile[0]));

                    IsInitialized = true;
                }
                return true;
            }
            catch (Exception)
            {
                // No se pudo crear el log
                return false;
            }
        }

        public static LogLevel ParseLogLevel(string logLevel)
        {
            if (string.IsNullOrWhiteSpace(logLevel))
                return LogLevel.Error;

            switch (logLevel.ToLower())
            {
                case "error":
                    return LogLevel.Error;

                case "debug":
                    return LogLevel.Debug;

                case "warning":
                    return LogLevel.Warning;

                default:
                    return LogLevel.Error;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Renombra los archivos de LOGs y crea uno nuevo
        /// </summary>
        /// <param name="p_strPath">Path al directorio de LOG</param>
        /// <param name="p_strBase">Base del Nombre del LOG</param>
        /// <param name="p_strSistema">Sigla del Sistema</param>
        private static void MakeNewLOGFile()
        {
            if (File.Exists(m_strLogFile[0]))
            {
                // Grabamos el cierre del archivo

                try
                {
                    m_textWriter = TextWriter.Synchronized(File.AppendText(m_strLogFile[0]));
                    m_textWriter.WriteLine("</table>");
                    m_textWriter.WriteLine("</body>");
                    m_textWriter.WriteLine("</html>");
                    m_textWriter.Flush();
                }
                catch (Exception)
                {
                }
                finally
                {
                    // si pude abrir el stream -> lo cierro
                    if (m_textWriter != null) m_textWriter.Close();
                    m_textWriter = null;
                }
            }

            // Borramos el ultimo archivo de LOG
            try
            {
                if (File.Exists(m_strLogFile[4])) File.Delete(m_strLogFile[4]);
            }
            catch (Exception)
            {
                return;
            }

            // Renombramos del 3 al base
            int l_iIdx = 0;

            try
            {
                for (l_iIdx = 3; l_iIdx >= 0; l_iIdx--)
                {
                    if (File.Exists(m_strLogFile[l_iIdx]))
                        File.Move(m_strLogFile[l_iIdx], m_strLogFile[l_iIdx + 1]);
                }
            }
            catch (Exception)
            {
                return;
            }

            // Grabamos el encabezado del archivo
            try
            {
                // Abrimos el archivo de log
                m_textWriter = TextWriter.Synchronized(File.AppendText(m_strLogFile[0]));

                m_textWriter.WriteLine("<html>");
                m_textWriter.WriteLine("<head>");
                m_textWriter.WriteLine("<meta http-equiv=\"Content-Type\"");
                m_textWriter.WriteLine("content=\"text/html; charset=iso-8859-1\">");
                m_textWriter.WriteLine("<title>Log File</title>");
                m_textWriter.WriteLine("</head>");
                m_textWriter.WriteLine("<body>");
                m_textWriter.WriteLine("<h1>Log</h1>");
                m_textWriter.WriteLine("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\">");
                m_textWriter.Flush();
            }
            catch
            {
                return;
            }
        }

        private static string ReplaceEnters(string msg)
        {
            return msg.Replace("\r\n", "<br>");
        }

        #endregion Private Methods
    }
}

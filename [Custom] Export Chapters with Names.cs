using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;
using ScriptPortal.Vegas;
using System.Xml;

//Custom Vegas Pro Script
//Created 2024\11\24 by Randy Turner, Version: 1.7 (2025\05\25)
//Open subroutines closed
//RulerFormat set to RulerFormat.Time (hh:mm:ss.fff)
//Export methods extended to include Marker: Index, Position, & Label
//Column Headers and Marker Index Optional for CSV & TXT

public class EntryPoint
{
	Vegas myVegas;
	DialogResult IncludeHeaders;
	DialogResult IncludeChapterIDs;

	public void FromVegas(Vegas vegas) {
		myVegas = vegas; 
		String projName;
		String projFile = myVegas.Project.FilePath;
		if (vegas.Project.Ruler.Format != RulerFormat.Time)	{
			vegas.Project.Ruler.Format = RulerFormat.Time;
			vegas.UpdateUI(); // Refresh the UI to reflect changes
            System.Windows.Forms.MessageBox.Show("Ruler Format Set to (Required) Time.", "Ruler Format Changed!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

		if (String.IsNullOrEmpty(projFile)) {
			projName = "Untitled";
		} else  {
			projName = Path.GetFileNameWithoutExtension(projFile);
		}

		String exportFile = ShowSaveFileDialog(BuildFormatString(),2,"Export Chapter Information", projName + "_Chapters");

        if (null != exportFile) {
			String ext = Path.GetExtension(exportFile).ToUpper();
			if ((null != ext) && (ext == ".XML"))
			{ExportChaptersToXML(exportFile);}
			else if ((null != ext) && (ext == ".TXT"))
			{ExportChaptersToTXT(exportFile);}
			else
			{ExportChaptersToCSV(exportFile);}
		}
	}
  
	StreamWriter CreateStreamWriter(String fileName, Encoding encoding) {
		FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
		StreamWriter sw = new StreamWriter(fs, encoding);
		return sw;
	}

	void WriteColumnHeaders(StreamWriter streamWriter)
	{
		StringBuilder hsv = new StringBuilder();
		if(IncludeChapterIDs == DialogResult.Yes){hsv.Append("ID\t");}
		hsv.Append("TimeCode\t");
		hsv.Append("Name");
		streamWriter.WriteLine(hsv.ToString());
	}

	void WriteColumnDetails(Marker marker, StreamWriter streamWriter)
	{
		StringBuilder tsv = new StringBuilder();
		if(IncludeChapterIDs == DialogResult.Yes){
			tsv.Append(marker.Index.ToString());
			tsv.Append('\t');};
		tsv.Append(marker.Position.ToString());
        tsv.Append('\t');
		tsv.Append(marker.Label.ToString());
		tsv.Append("\r\n");
		streamWriter.Write(tsv); //Write line
	}

	void ExportChaptersToCSV(String exportFile)
	{
        ExportChaptersToTXT(exportFile); //A CSV file is a text file.
    }

	void ExportChaptersToTXT(String exportFile) {
		StreamWriter streamWriter = null;
		IncludeHeaders = System.Windows.Forms.MessageBox.Show("Include Headers?","Input Parameters", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
		IncludeChapterIDs = System.Windows.Forms.MessageBox.Show("Include Chapter ID?", "Input Parameters", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
		try {
			streamWriter = CreateStreamWriter(exportFile, System.Text.Encoding.Unicode);
			if(IncludeHeaders == DialogResult.Yes){ WriteColumnHeaders(streamWriter); };
			foreach (Marker marker in myVegas.Project.Markers) {
				WriteColumnDetails(marker, streamWriter);
			}
		} finally {
			if (null != streamWriter)
			{
				streamWriter.Close();
				System.Windows.Forms.MessageBox.Show("Export successful. File name: " + exportFile, "Chapter File Export");
			}
		}		
	}

	void ExportChaptersToXML(String exportFile)
	{
		XmlDocument doc = null;
		try
		{
			doc = new XmlDocument();
			XmlProcessingInstruction xmlPI = doc.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
			doc.AppendChild(xmlPI);
			XmlElement root = doc.CreateElement("Chapters");
			System.Text.Encoding myCharacterEncoding = System.Text.Encoding.UTF8;
			doc.AppendChild(root);
			XmlElement chapter;
			foreach (Marker marker in myVegas.Project.Markers)
			{
				chapter = doc.CreateElement("chapter");
				chapter.SetAttribute("ID", marker.Index.ToString());
				chapter.SetAttribute("TimeCode", marker.Position.ToString()); 
				chapter.SetAttribute("Name", marker.Label); 
				root.AppendChild(chapter);
			}
			XmlTextWriter writer = new XmlTextWriter(exportFile, myCharacterEncoding);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;
			writer.IndentChar = ' ';
			doc.WriteTo(writer);
			writer.Close();
		}
		catch {
			doc = null;
		}
		finally
		{
			if (null != doc)
			{
				System.Windows.Forms.MessageBox.Show("Export successful. File name: " + exportFile, "Chapter File Export");
			}
		} 
	}
 
	String ShowSaveFileDialog(String filter, Int32 filterIndex, String title, String defaultFilename) {
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		if (null == filter) {
			filter = "All Files (*.*)|*.*";
			filterIndex = 1;
		}
		saveFileDialog.Filter = filter;
		saveFileDialog.FilterIndex = filterIndex;
		if (null != title)
			saveFileDialog.Title = title;
		saveFileDialog.CheckPathExists = true;
		saveFileDialog.AddExtension = true;
		if (null != defaultFilename) {
			String initialDir = Path.GetDirectoryName(defaultFilename);
			if (Directory.Exists(initialDir)) {
				saveFileDialog.InitialDirectory = initialDir;
			}
			saveFileDialog.DefaultExt = Path.GetExtension(defaultFilename);
			saveFileDialog.FileName = Path.GetFileName(defaultFilename);
		}
		if (System.Windows.Forms.DialogResult.OK == saveFileDialog.ShowDialog()) {
			return Path.GetFullPath(saveFileDialog.FileName);
		} else {
			return null;
		}
	}

	string BuildFormatString()
	{
		String[] Extensions = {"csv","txt","xml"};
		String mask = "<t> Chapter List (*.<e>)|*.<e>|";
		String fmtStr = "";
		foreach (String ext in Extensions)
			{
				String ws = mask;
				ws = ws.Replace("<t>",ext.ToUpper());
				ws = ws.Replace("<e>",ext);
				fmtStr += ws;
			}
		return fmtStr.Substring(0,fmtStr.Length - 1);
    }
}
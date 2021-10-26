using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sfs2X.Entities.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using Image = UnityEngine.UI.Image;

public class PDFmanager : MonoBehaviour
{
    //------------------------
    //VARIABLES
    //------------------------
    //Name of the proyect
    public InputField Titulo;
    string path = null;
    //path of the image
    string path_image4 = null;
    string path_image1 = null;
    string path_image2 = null;
    string path_image3 = null;
    string path_image5 = null;
    string path_icon = null;
    //PLayer for the cameras
    public Player photo;
    byte[] fileData;
    public PackageManajer paquetes;
    public IrregularManager ir;
    //Cam of the loading sequence
    public SnapShot SnapCam5;
    //Text of indicators
    public Text Vol_T;
    public Text Vol_free;
    public Text Peso;
    public GameObject contenedor;
    public UnityEngine.UI.Image icon;
    public RectTransform parent;
    //Pdf Bar loading
    public Image LoadBarPdf;
    private bool loading_bar_pdf = false;
    private float TimeLeft;
    //Language
    public GlobalLanguage globalLa;

    //------------------------
    //METHODS
    //------------------------
   

     void Update()
    {
        //Acive the loading bar
        if (loading_bar_pdf)
        {
            if (TimeLeft < 2)
            {
                TimeLeft += Time.deltaTime;
                LoadBarPdf.fillAmount = TimeLeft / 2;
            }
            else
            {
                LoadBarPdf.enabled = false;
            }
        }
    }
    //------------------
    //Create the file on the path
    //------------------
    public async void GenerateFile()
    {
        if (!paquetes.activeFuntion)
        {
            if (globalLa.returnLanguage() == "Español")
            {
                GenerateSpanish();
            }
            else if (globalLa.returnLanguage() == "English")
            {
                GenerateEnglish();
            }
        }
    }
    public async void GenerateSpanish()
    {
            try
            {
                paquetes.activeFuntion = true;
                //Validate if  package exist
                paquetes.ArePackage();

                //Active loading bar 
                LoadBarPdf.enabled = true;
                TimeLeft = 0;
                loading_bar_pdf = true;

                //path of pdf of the proyect
                path = String.Format("{0}/StreamingAssets/pdf/{1}.pdf", Application.dataPath, Titulo.text);

                PdfPTable table = new PdfPTable(3);
                PdfPCell cell = new PdfPCell();

                //Take  side photos
                photo.TomarSnapShot();

                SnapCam5.callTakeSnapShot();
                Debug.Log("inicio");
                await Task.Delay(3000);
                Debug.Log("fin");

                //Call the image 
                path_image1 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_1.png";
                path_image2 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_2.png";
                path_image3 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_3.png";
                path_image4 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_4.png";

                //Validate if the file exist
                if (File.Exists(path))
                    File.Delete(path);

                //Create the file
                using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {



                    var document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    var writer = PdfWriter.GetInstance(document, fileStream);
                    document.Open();
                    document.NewPage();


                    var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    Paragraph p = new Paragraph(string.Format("Reporte de resultados para el escenario: {0}", Titulo.text)); //iSFSObject.GetUtfString("TICKET_ID"
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);

                    p = new Paragraph(string.Format("El volumen actual es : {0}      El volumen libre es: {1}    El peso total es: {2}  ", Vol_T.text, Vol_free.text, Peso.text));
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);
                    p = new Paragraph(string.Format("Container: {0} m X {1} m X {2} m", contenedor.transform.localScale.x, contenedor.transform.localScale.y, contenedor.transform.localScale.z));
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);
                    p = new Paragraph("");
                    document.Add(p);
                    p = new Paragraph("_______________________________________________________________________________");
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);

                    iTextSharp.text.Image SnapShot1 = iTextSharp.text.Image.GetInstance(path_image1);
                    iTextSharp.text.Image SnapShot2 = iTextSharp.text.Image.GetInstance(path_image2);
                    iTextSharp.text.Image SnapShot3 = iTextSharp.text.Image.GetInstance(path_image3);
                    iTextSharp.text.Image SnapShot4 = iTextSharp.text.Image.GetInstance(path_image4);

                    //Scale the image
                    SnapShot1.ScaleToFit(160f, 140f);
                    SnapShot2.ScaleToFit(200f, 180f);
                    SnapShot3.ScaleToFit(160f, 140f);
                    SnapShot4.ScaleToFit(160f, 140f);

                    p = new Paragraph();
                    p.Add(new Chunk(SnapShot2, 0, 0));
                    p.Alignment = Element.ALIGN_CENTER;
                    p.SetLeading(0, 16f);
                    document.Add(p);

                    p = new Paragraph("Vista puerta de carga");
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);

                    //Insert the side photos
                    p = new Paragraph();
                    p.Add(new Chunk(SnapShot1, 0, 0));
                    p.Add(new Phrase("         "));
                    p.Add(new Chunk(SnapShot3, 0, 0));
                    p.Add(new Phrase("         "));
                    p.Add(new Chunk(SnapShot4, 0, 0));
                    p.Alignment = Element.ALIGN_CENTER;
                    p.SetLeading(0, 14f);
                    document.Add(p);
                    p = new Paragraph("Vista lado izquierda                        Vista  Superior                        Vista lado derecho");
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);
                    p = new Paragraph("_______________________________________________________________________________");
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);
                    p = new Paragraph("Paquetes");
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);

                    //Insert the info of the items
                    foreach (var item in paquetes.pTypes)
                    {
                        path_icon = string.Format("{0}/StreamingAssets/Colors/{1}.png", Application.dataPath, item.Value.packageId);
                        iTextSharp.text.Image icon = iTextSharp.text.Image.GetInstance(path_icon);
                        icon.ScaleToFit(10f, 10f);
                        //icon.GrayFill

                        p = new Paragraph();
                        p.Add(new Phrase("    "));
                        p.Add(new Chunk(icon, 0, 0));
                        p.Add(new Phrase(string.Format(" {0} Cantidad: {1}  Cliente: {2}  Peso: {3}  L: {4}  W: {5}  H: {6} ",
                            item.Value.Name1,
                            item.Value.quantity,
                            item.Value.clientId,
                            item.Value.weight,
                            item.Value.packageSize.x,
                            item.Value.packageSize.y,
                            item.Value.packageSize.z
                            )));
                        //   p.Add(new Chunk(icon, 0, 0));
                        document.Add(p);
                        p = new Paragraph("");
                        document.Add(p);
                    }



                    p = new Paragraph("_______________________________________________________________________________");
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);
                    p = new Paragraph("Orden de carga");
                    p.Alignment = Element.ALIGN_CENTER;
                    document.Add(p);
                    ArrayList mymessage = new ArrayList();

                    //Insert photos of the groups in the order
                    for (int i = 1; i <= paquetes.numGroups; i++)
                    {
                        for (int j = 0; j < paquetes.pTypes.Count; j++)
                        {
                            int item = 0;
                            foreach (Transform child in parent)
                            {
                                int groupId;
                                int packageId;
                                GameObject go = child.gameObject;
                                groupId = go.GetComponent<Package>().groupId;
                                packageId = go.GetComponent<Package>().itemId;
                                if (groupId == i && packageId == j)
                                {
                                    item++;
                                }

                            }
                            if (item > 0)
                            {
                                Debug.Log(j);
                                string namePackage = paquetes.pTypes[j].Name1;
                                mymessage.Add(namePackage + ": " + item);
                            }
                        }

                        path_image5 = string.Format("{0}/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_5_{1}.png",
                        Application.dataPath,
                         i);
                        Debug.Log(path_image5);

                        iTextSharp.text.Image SnapShot5 = iTextSharp.text.Image.GetInstance(path_image5);

                        SnapShot5.ScaleToFit(200f, 180f);


                        //Group with even number
                        if ((i % 2) == 0)
                        {

                            p.Add(new Chunk(SnapShot5, 0, 0));
                            p.Alignment = Element.ALIGN_CENTER;
                            p.SetLeading(0, 16f);
                            document.Add(p);
                            p = new Paragraph("                              " + "Grupo de cargue " + (i - 1) + "                                   " + "Grupo de cargue " + (i));

                            document.Add(p);
                            p = new Paragraph("                              " + mymessage[i - 2] + "                                                    " + mymessage[i - 1]);

                            document.Add(p);
                        }
                        //if it`s the lastone
                        else if (i == paquetes.numGroups)
                        {
                            p = new Paragraph();
                            p.Add(new Chunk(SnapShot5, 0, 0));
                            p.Alignment = Element.ALIGN_CENTER;
                            p.SetLeading(0, 16f);
                            document.Add(p);
                            p = new Paragraph("Grupo de cargue " + (i));
                            p.Alignment = Element.ALIGN_CENTER;
                            document.Add(p);
                            p = new Paragraph("" + mymessage[i - 1]);
                            p.Alignment = Element.ALIGN_CENTER;
                            document.Add(p);
                        }
                        else
                        {
                            p = new Paragraph();
                            p.Add(new Chunk(SnapShot5, 0, 0));
                            p.Add(new Phrase("         "));
                        }

                        /**
                       **/
                    }


                    document.Close();
                    writer.Close();
                }

                PrintFiles();
                paquetes.activeFuntion = false;
            }
            catch (System.Exception)
            {
                Debug.Log("callo en error");
                return;
            }
        
    }
    public async void GenerateEnglish()
    {
        try
        {
            paquetes.activeFuntion = true;
            //Validate if  package exist
            paquetes.ArePackage();

            //Active loading bar 
            LoadBarPdf.enabled = true;
            TimeLeft = 0;
            loading_bar_pdf = true;

            //path of pdf of the proyect
            path = String.Format("{0}/StreamingAssets/pdf/{1}.pdf", Application.dataPath, Titulo.text);

            PdfPTable table = new PdfPTable(3);
            PdfPCell cell = new PdfPCell();

            //Take  side photos
            photo.TomarSnapShot();

            SnapCam5.callTakeSnapShot();
            Debug.Log("inicio");
            await Task.Delay(3000);
            Debug.Log("fin");

            //Call the image 
            path_image1 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_1.png";
            path_image2 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_2.png";
            path_image3 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_3.png";
            path_image4 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_4.png";

            //Validate if the file exist
            if (File.Exists(path))
                File.Delete(path);

            //Create the file
            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {



                var document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                var writer = PdfWriter.GetInstance(document, fileStream);
                document.Open();
                document.NewPage();


                var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Paragraph p = new Paragraph(string.Format("Results report for the scenario:{0}", Titulo.text)); //iSFSObject.GetUtfString("TICKET_ID"
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                p = new Paragraph(string.Format("The current volume is: {0} The free volume is: {1} The total weight is: {2} ", Vol_T.text, Vol_free.text, Peso.text));
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph(string.Format("Container: {0} m X {1} m X {2} m", contenedor.transform.localScale.x, contenedor.transform.localScale.y, contenedor.transform.localScale.z));
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("");
                document.Add(p);
                p = new Paragraph("_______________________________________________________________________________");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                iTextSharp.text.Image SnapShot1 = iTextSharp.text.Image.GetInstance(path_image1);
                iTextSharp.text.Image SnapShot2 = iTextSharp.text.Image.GetInstance(path_image2);
                iTextSharp.text.Image SnapShot3 = iTextSharp.text.Image.GetInstance(path_image3);
                iTextSharp.text.Image SnapShot4 = iTextSharp.text.Image.GetInstance(path_image4);

                //Scale the image
                SnapShot1.ScaleToFit(160f, 140f);
                SnapShot2.ScaleToFit(200f, 180f);
                SnapShot3.ScaleToFit(160f, 140f);
                SnapShot4.ScaleToFit(160f, 140f);

                p = new Paragraph();
                p.Add(new Chunk(SnapShot2, 0, 0));
                p.Alignment = Element.ALIGN_CENTER;
                p.SetLeading(0, 16f);
                document.Add(p);

                p = new Paragraph("Loading door view");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                //Insert the side photos
                p = new Paragraph();
                p.Add(new Chunk(SnapShot1, 0, 0));
                p.Add(new Phrase("         "));
                p.Add(new Chunk(SnapShot3, 0, 0));
                p.Add(new Phrase("         "));
                p.Add(new Chunk(SnapShot4, 0, 0));
                p.Alignment = Element.ALIGN_CENTER;
                p.SetLeading(0, 14f);
                document.Add(p);
                p = new Paragraph("Left side view                        Top view                        Right side view");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("_______________________________________________________________________________");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("Packages");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                //Insert the info of the items
                foreach (var item in paquetes.pTypes)
                {
                    path_icon = string.Format("{0}/StreamingAssets/Colors/{1}.png", Application.dataPath, item.Value.packageId);
                    iTextSharp.text.Image icon = iTextSharp.text.Image.GetInstance(path_icon);
                    icon.ScaleToFit(10f, 10f);
                    //icon.GrayFill

                    p = new Paragraph();
                    p.Add(new Phrase("    "));
                    p.Add(new Chunk(icon, 0, 0));
                    p.Add(new Phrase(string.Format(" {0} Quantity: {1} Client: {2} Weight: {3}  L: {4}  W: {5}  H: {6} ",
                        item.Value.Name1,
                        item.Value.quantity,
                        item.Value.clientId,
                        item.Value.weight,
                        item.Value.packageSize.x,
                        item.Value.packageSize.y,
                        item.Value.packageSize.z
                        )));
                    //   p.Add(new Chunk(icon, 0, 0));
                    document.Add(p);
                    p = new Paragraph("");
                    document.Add(p);
                }



                p = new Paragraph("_______________________________________________________________________________");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("Loading order");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                ArrayList mymessage = new ArrayList();

                //Insert photos of the groups in the order
                for (int i = 1; i <= paquetes.numGroups; i++)
                {
                    for (int j = 0; j < paquetes.pTypes.Count; j++)
                    {
                        int item = 0;
                        foreach (Transform child in parent)
                        {
                            int groupId;
                            int packageId;
                            GameObject go = child.gameObject;
                            groupId = go.GetComponent<Package>().groupId;
                            packageId = go.GetComponent<Package>().itemId;
                            if (groupId == i && packageId == j)
                            {
                                item++;
                            }

                        }
                        if (item > 0)
                        {
                            Debug.Log(j);
                            string namePackage = paquetes.pTypes[j].Name1;
                            mymessage.Add(namePackage + ": " + item);
                        }
                    }

                    path_image5 = string.Format("{0}/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_5_{1}.png",
                    Application.dataPath,
                     i);
                    Debug.Log(path_image5);

                    iTextSharp.text.Image SnapShot5 = iTextSharp.text.Image.GetInstance(path_image5);

                    SnapShot5.ScaleToFit(200f, 180f);


                    //Group with even number
                    if ((i % 2) == 0)
                    {

                        p.Add(new Chunk(SnapShot5, 0, 0));
                        p.Alignment = Element.ALIGN_CENTER;
                        p.SetLeading(0, 16f);
                        document.Add(p);
                        p = new Paragraph("                              " + "Load group " + (i - 1) + "                                   " + "Load group " + (i));

                        document.Add(p);
                        p = new Paragraph("                              " + mymessage[i - 2] + "                                                    " + mymessage[i - 1]);

                        document.Add(p);
                    }
                    //if it`s the lastone
                    else if (i == paquetes.numGroups)
                    {
                        p = new Paragraph();
                        p.Add(new Chunk(SnapShot5, 0, 0));
                        p.Alignment = Element.ALIGN_CENTER;
                        p.SetLeading(0, 16f);
                        document.Add(p);
                        p = new Paragraph("Load group" + (i));
                        p.Alignment = Element.ALIGN_CENTER;
                        document.Add(p);
                        p = new Paragraph("" + mymessage[i - 1]);
                        p.Alignment = Element.ALIGN_CENTER;
                        document.Add(p);
                    }
                    else
                    {
                        p = new Paragraph();
                        p.Add(new Chunk(SnapShot5, 0, 0));
                        p.Add(new Phrase("         "));
                    }

                    /**
                   **/
                }


                document.Close();
                writer.Close();
            }

            PrintFiles();
            paquetes.activeFuntion = false;
        }
        catch (System.Exception)
        {
            Debug.Log("callo en error");
            return;
        }

    }
    public async void GenerateFileIrregular()
    {
        if (globalLa.returnLanguage() == "Español")
        {
            GenerateSpanishIrregular();
        }
        else if (globalLa.returnLanguage() == "English")
        {
            GenerateEnglishIrregular();
        }

    }
    public async void GenerateSpanishIrregular()
    {
        try
        {
            //paquetes.activeFuntion = true;
            //Validate if  package exist
            //paquetes.ArePackage();

            //Active loading bar 
            LoadBarPdf.enabled = true;
            TimeLeft = 0;
            loading_bar_pdf = true;

            //path of pdf of the proyect
            path = String.Format("{0}/StreamingAssets/OtherObjects/PDF/{1}.pdf", Application.dataPath, Titulo.text);

            PdfPTable table = new PdfPTable(3);
            PdfPCell cell = new PdfPCell();

            //Take  side photos
            photo.TomarSnapShot();

            SnapCam5.callTakeSnapShot();
            Debug.Log("inicio");
            await Task.Delay(3000);
            Debug.Log("fin");

            //Call the image 
            path_image1 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_6.png";
            path_image2 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_2.png";
            path_image3 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_3.png";
            path_image4 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_4.png";

            //Validate if the file exist
            if (File.Exists(path))
                File.Delete(path);

            //Create the file
            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                var writer = PdfWriter.GetInstance(document, fileStream);
                document.Open();
                document.NewPage();


                var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Paragraph p = new Paragraph(string.Format("Reporte de resultados para el escenario: {0}", Titulo.text)); //iSFSObject.GetUtfString("TICKET_ID"
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                p = new Paragraph(string.Format("El volumen actual es : {0}      El volumen libre es: {1}    El peso total es: {2}  ", Vol_T.text, Vol_free.text, Peso.text));
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph(string.Format("Container: {0} m X {1} m X {2} m", contenedor.transform.localScale.x, contenedor.transform.localScale.y, contenedor.transform.localScale.z));
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("");
                document.Add(p);
                p = new Paragraph("_______________________________________________________________________________");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                iTextSharp.text.Image SnapShot1 = iTextSharp.text.Image.GetInstance(path_image1);
                iTextSharp.text.Image SnapShot2 = iTextSharp.text.Image.GetInstance(path_image2);
                iTextSharp.text.Image SnapShot3 = iTextSharp.text.Image.GetInstance(path_image3);
                iTextSharp.text.Image SnapShot4 = iTextSharp.text.Image.GetInstance(path_image4);

                //Scale the image
                SnapShot1.ScaleToFit(160f, 140f);
                SnapShot2.ScaleToFit(160f, 140f);
                SnapShot3.ScaleToFit(160f, 140f);
                SnapShot4.ScaleToFit(200f, 180f);

                p = new Paragraph();
                p.Add(new Chunk(SnapShot4, 0, 0));
                p.Alignment = Element.ALIGN_CENTER;
                p.SetLeading(0, 16f);
                document.Add(p);

                p = new Paragraph("Vista puerta de carga");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                //Insert the side photos
                p = new Paragraph();
                p.Add(new Chunk(SnapShot2, 0, 0));
                p.Add(new Phrase("         "));
                p.Add(new Chunk(SnapShot3, 0, 0));
                p.Add(new Phrase("         "));
                p.Add(new Chunk(SnapShot1, 0, 0));
                p.Alignment = Element.ALIGN_CENTER;
                p.SetLeading(0, 14f);
                document.Add(p);
                p = new Paragraph("Vista lado izquierda                        Vista  Superior                        Vista lado derecho");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("_______________________________________________________________________________");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("Paquetes");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                //Insert the info of the items
                foreach (var item in ir.pTypesIR)
                {
                    path_icon = string.Format("{0}/StreamingAssets/Colors/{1}.png", Application.dataPath, item.Value.packageId);
                    iTextSharp.text.Image icon = iTextSharp.text.Image.GetInstance(path_icon);
                    icon.ScaleToFit(10f, 10f);
                    //icon.GrayFill

                    p = new Paragraph();
                    p.Add(new Phrase("    "));
                    p.Add(new Chunk(icon, 0, 0));
                    string type;
                    if (item.Value.classIR == 1)
                    {
                        type = "Caja";
                        p.Add(new Phrase(string.Format(" {0} ({1}) Cantidad: {2}  Peso: {3} L: {4}  W: {5}  H: {6}",
                       item.Value.Name1,
                       type,
                       item.Value.quantity,
                       item.Value.weight,
                       item.Value.packageSize.x,
                       item.Value.packageSize.y,
                       item.Value.packageSize.z
                       )));
                    }
                    else if (item.Value.classIR == 2)
                    {
                        type = "Cilindro";
                        p.Add(new Phrase(string.Format(" {0} ({1}) Cantidad: {2}  Peso: {3}  H: {4}  R: {5}",
                       item.Value.Name1,
                       type,
                       item.Value.quantity,
                       item.Value.weight,
                       item.Value.packageSize.y,
                       item.Value.packageSize.z
                       )));
                    }
                    else
                    {
                        type = "Otro";
                        p.Add(new Phrase(string.Format(" {0} ({1}) Cantidad: {2}  Peso: {3}",
                       item.Value.Name1,
                       type,
                       item.Value.quantity,
                       item.Value.weight
                       )));
                    }

                    //   p.Add(new Chunk(icon, 0, 0));
                    document.Add(p);
                    p = new Paragraph("");
                    document.Add(p);
                }


                document.Close();
                writer.Close();
            }

            PrintFiles();
            paquetes.activeFuntion = false;
        }
        catch (System.Exception)
        {
            Debug.Log("callo en error");
            return;
        }

    }
    public async void GenerateEnglishIrregular()
    {
        try
        {
            //paquetes.activeFuntion = true;
            //Validate if  package exist
            //paquetes.ArePackage();

            //Active loading bar 
            LoadBarPdf.enabled = true;
            TimeLeft = 0;
            loading_bar_pdf = true;

            //path of pdf of the proyect
            path = String.Format("{0}/StreamingAssets/OtherObjects/PDF/{1}.pdf", Application.dataPath, Titulo.text);

            PdfPTable table = new PdfPTable(3);
            PdfPCell cell = new PdfPCell();

            //Take  side photos
            photo.TomarSnapShot();

            SnapCam5.callTakeSnapShot();
            Debug.Log("inicio");
            await Task.Delay(3000);
            Debug.Log("fin");

            //Call the image 
            path_image1 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_6.png";
            path_image2 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_2.png";
            path_image3 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_3.png";
            path_image4 = Application.dataPath + "/StreamingAssets/SnapShots/snap_1080x1080_SnapShot_Camara_4.png";

            //Validate if the file exist
            if (File.Exists(path))
                File.Delete(path);

            //Create the file
            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var document = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                var writer = PdfWriter.GetInstance(document, fileStream);
                document.Open();
                document.NewPage();


                var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Paragraph p = new Paragraph(string.Format("Results report for the scenario: {0}", Titulo.text)); //iSFSObject.GetUtfString("TICKET_ID"
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                p = new Paragraph(string.Format("The current volume is: {0} The free volume is: {1} The total weight is: {2}", Vol_T.text, Vol_free.text, Peso.text));
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph(string.Format("Container: {0} m X {1} m X {2} m", contenedor.transform.localScale.x, contenedor.transform.localScale.y, contenedor.transform.localScale.z));
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("");
                document.Add(p);
                p = new Paragraph("_______________________________________________________________________________");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                iTextSharp.text.Image SnapShot1 = iTextSharp.text.Image.GetInstance(path_image1);
                iTextSharp.text.Image SnapShot2 = iTextSharp.text.Image.GetInstance(path_image2);
                iTextSharp.text.Image SnapShot3 = iTextSharp.text.Image.GetInstance(path_image3);
                iTextSharp.text.Image SnapShot4 = iTextSharp.text.Image.GetInstance(path_image4);

                //Scale the image
                SnapShot1.ScaleToFit(160f, 140f);
                SnapShot2.ScaleToFit(160f, 140f);
                SnapShot3.ScaleToFit(160f, 140f);
                SnapShot4.ScaleToFit(200f, 180f);

                p = new Paragraph();
                p.Add(new Chunk(SnapShot4, 0, 0));
                p.Alignment = Element.ALIGN_CENTER;
                p.SetLeading(0, 16f);
                document.Add(p);

                p = new Paragraph("Loading door view");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                //Insert the side photos
                p = new Paragraph();
                p.Add(new Chunk(SnapShot2, 0, 0));
                p.Add(new Phrase("         "));
                p.Add(new Chunk(SnapShot3, 0, 0));
                p.Add(new Phrase("         "));
                p.Add(new Chunk(SnapShot1, 0, 0));
                p.Alignment = Element.ALIGN_CENTER;
                p.SetLeading(0, 14f);
                document.Add(p);
                p = new Paragraph("Left side view                        Top view                        Right side view");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("_______________________________________________________________________________");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);
                p = new Paragraph("Packages");
                p.Alignment = Element.ALIGN_CENTER;
                document.Add(p);

                //Insert the info of the items
                foreach (var item in ir.pTypesIR)
                {
                    path_icon = string.Format("{0}/StreamingAssets/Colors/{1}.png", Application.dataPath, item.Value.packageId);
                    iTextSharp.text.Image icon = iTextSharp.text.Image.GetInstance(path_icon);
                    icon.ScaleToFit(10f, 10f);
                    //icon.GrayFill

                    p = new Paragraph();
                    p.Add(new Phrase("    "));
                    p.Add(new Chunk(icon, 0, 0));
                    string type;
                    if (item.Value.classIR == 1)
                    {
                        type = "Cube";
                        p.Add(new Phrase(string.Format("{0} ({1}) Quantity: {2} Weight: {3} L: {4} W: {5} H: {6}",
                       item.Value.Name1,
                       type,
                       item.Value.quantity,
                       item.Value.weight,
                       item.Value.packageSize.x,
                       item.Value.packageSize.y,
                       item.Value.packageSize.z
                       )));
                    }
                    else if (item.Value.classIR == 2)
                    {
                        type = "Cylinder";
                        p.Add(new Phrase(string.Format(" {0} ({1}) Quantity: {2} Weight: {3} H: {4} R: {5}",
                       item.Value.Name1,
                       type,
                       item.Value.quantity,
                       item.Value.weight,
                       item.Value.packageSize.y,
                       item.Value.packageSize.z
                       )));
                    }
                    else
                    {
                        type = "Other";
                        p.Add(new Phrase(string.Format(" {0} ({1}) Quantity: {2} Weight: {3}",
                       item.Value.Name1,
                       type,
                       item.Value.quantity,
                       item.Value.weight
                       )));
                    }

                    //   p.Add(new Chunk(icon, 0, 0));
                    document.Add(p);
                    p = new Paragraph("");
                    document.Add(p);
                }


                document.Close();
                writer.Close();
            }

            PrintFiles();
            paquetes.activeFuntion = false;
        }
        catch (System.Exception)
        {
            Debug.Log("callo en error");
            return;
        }

    }

    //------------------
    //Open the pdf 
    //------------------
    void PrintFiles()
    {
        Debug.Log(path);
        if (path == null)
            return;

        //VCalidate if the file exist
        if (File.Exists(path))
        {
            Debug.Log("file found");
       
        }
        else
        {
            Debug.Log("file not found");
            return;
        }

        //Open the path with the pdf
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = path;
        process.Start();

    }
}

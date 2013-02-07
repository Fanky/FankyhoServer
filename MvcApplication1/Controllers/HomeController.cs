using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SpeechLib;
using System.Collections;
using System.Diagnostics;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {

        int accuracyLimit = 0;
        float accuracyMax = 0.1F; //to avoid divide by zero
        string firstRecognition = "";
        private SpeechLib.SpSharedRecoContext objRecoContext;
        ISpeechGrammarRule rule;
        ISpeechGrammarRuleState state;
        ISpeechRecoGrammar grammar;
        SpVoice ob;
        private void initSAPI()//STARTS THE RECOGNITION ENGINE
        {
            try
            {
                objRecoContext = new SpeechLib.SpSharedRecoContext();
                objRecoContext.AudioLevel +=
                    new _ISpeechRecoContextEvents_AudioLevelEventHandler(RecoContext_VUMeter);
                objRecoContext.Recognition +=
                    new _ISpeechRecoContextEvents_RecognitionEventHandler(RecoContext_Recognition);
                objRecoContext.EventInterests = SpeechLib.SpeechRecoEvents.SRERecognition |
                    SpeechLib.SpeechRecoEvents.SREAudioLevel;
                // objRecoContext.StartStream += new _ISpeechRecoContextEvents_StartStreamEventHandler(RecoContext_StartStream);


                //create grammar interface with ID = 0
                grammar = objRecoContext.CreateGrammar(0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //Label1.Text = "Exeption (Init SAPI)\n" + ex.ToString();
            }
        }
        /*VOLUME METER*/
        public void RecoContext_VUMeter(int StreamNumber, object StreamPosition, int e)
        {
            Debug.WriteLine("Volumen: " + e.ToString());
            //Label2.Text = "Volumen: " + e.ToString();
        }
        /*ADDS WORDS TO THE GRAMMAR*/
        private void SAPIGrammarFromArrayList(ArrayList phraseList)
        {
            object propertyValue = "";
            int i;
            for (i = 0; i < phraseList.Count; i++)
            {
                rule = grammar.Rules.Add(phraseList[i].ToString(), SpeechRuleAttributes.SRATopLevel, i + 100);


                //add new word to the rule
                state = rule.InitialState;
                propertyValue = "";
                //state.AddWordTransition(null, command1.phrase, " ",
                //    SpeechGrammarWordType.SGLexical, "",
                //    0, ref propertyValue, 1F);
                state.AddWordTransition(null, phraseList[i].ToString(), " ",
                    SpeechGrammarWordType.SGLexical, "",
                    0, ref propertyValue, 1F);
                //commit rules
                grammar.Rules.Commit();


                //make rule active (needed for each rule)
                grammar.CmdSetRuleState(phraseList[i].ToString(), SpeechRuleState.SGDSActive);
            }
        }
        /*EVENT TRIGGERED IN A RECOGNITION EVENT*/
        public void RecoContext_Recognition(int StreamNumber, object StreamPosition,
            SpeechRecognitionType RecognitionType, ISpeechRecoResult e)
        {
            //get phrase
            string phrase = e.PhraseInfo.GetText(0, -1, true);
            Debug.WriteLine(phrase);

        }

        protected void Button1_Click()
        {
            //THIS BLOCK WORKS RIGHT
            SpVoice ob = new SpVoice();
            ob.Rate = -100;
            ob.Speak("Start recognistion", SpeechVoiceSpeakFlags.SVSFDefault);
            SpStream str = new SpStream();
        }


        protected void Button2_Click()
        {
            //THIS ONE DOESN'T
            ArrayList vec = new ArrayList(10);
            initSAPI();//STARTS THE RECOGNITION ENGINE
            SAPIGrammarFromArrayList(vec);
            grammar.State = SpeechLib.SpeechGrammarState.SGSEnabled;
            grammar.DictationLoad(null, SpeechLoadOption.SLOStatic);
            grammar.DictationSetState(SpeechRuleState.SGDSActive);
        }

        static void Ma()
        {

            SpSharedRecoContext ssrc = new SpSharedRecoContext();
            SpeechLib.ISpeechRecoGrammar isrg = null;

     //       ssrc = // SpSharedRecoContextClass();


            ssrc.Recognition += new _ISpeechRecoContextEvents_RecognitionEventHandler(RecognitionEvent);

            isrg = ssrc.CreateGrammar(1);
            isrg.DictationLoad(null, SpeechLib.SpeechLoadOption.SLOStatic);
            isrg.DictationSetState(SpeechLib.SpeechRuleState.SGDSActive);


        }

        public static void RecognitionEvent(int i, object o, SpeechLib.SpeechRecognitionType srt, SpeechLib.ISpeechRecoResult isrr)
        {
            string strText = isrr.PhraseInfo.GetText(0, -1, true);
            Console.WriteLine("recognized: " + strText);
        }

        public ActionResult Index()
        {
            Ma();
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

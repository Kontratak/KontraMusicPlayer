package com.example.kontramusicplayer;

import eneter.messaging.diagnostic.EneterTrace;
import eneter.messaging.endpoints.typedmessages.*;
import eneter.messaging.messagingsystems.messagingsystembase.*;
import eneter.messaging.messagingsystems.tcpmessagingsystem.TcpMessagingSystemFactory;
import eneter.net.system.EventHandler;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.*;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Timer;
import java.util.TimerTask;


public class MainActivity extends Activity
{
    // Request message type
    // The message must have the same name as declared in the service.
    // Also, if the message is the inner class, then it must be static.
    public static class MyRequest
    {
        public String Text;
    }

    // Response message type
    // The message must have the same name as declared in the service.
    // Also, if the message is the inner class, then it must be static.
    public static class MyResponse
    {
        public String Text;
        public int Length;
    }

    // UI controls
    final Handler handler = new Handler();
    private Handler myRefresh = new Handler();
    private EditText myMassageEditText;
    public TextView infoip;
    public TextView myMusicName;
    private Button mySendRequestBtn;
    private Button myResumePauseRequestBtn;
    private Button myNextRequestBtn;
    private Button myPrevRequestBtn;
    private Button myplaylistBtn;
    private Button myaddplBtn;
    private Button mytenforBtn;
    private Button mytenbackBtn;
    private Button myreplayBtn;
    ImageView imgview;
    float s;
    private int connectclicktimes = 0;
    private int pauseclicktimes = 0;
    String musicname;
    String portnum = "8060";
    public final static String EXTRA_MESSAGE = "com.example.kontramusicplayer.MESSAGE";
    Timer timer;
    TimerTask timerTask;
    String serverip = null;
    boolean replay = false;
    private IDuplexTypedMessageSender<MyResponse, MyRequest> mySender;
    File f = new File("/data/data/com.example.kontramusicplayer/files/musics.mpl");

    // Sender sending MyRequest and as a response receiving MyResponse.it

     /** Called when the activy is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Get UI widgets.
        mySendRequestBtn = (Button) findViewById(R.id.sendRequestBtn);
        myResumePauseRequestBtn = (Button) findViewById(R.id.pauseRequestBtn);
        myNextRequestBtn = (Button) findViewById(R.id.nextRequestBtn);
        myPrevRequestBtn = (Button) findViewById(R.id.prevRequestBtn);
        myMassageEditText = (EditText) findViewById(R.id.messageTextEditText);
        infoip = (TextView) findViewById(R.id.editText);
        myMusicName = (TextView) findViewById(R.id.musicName);
        imgview = (ImageView)findViewById(R.id.imageView5);
        myaddplBtn = (Button) findViewById(R.id.addplbtn);
        myplaylistBtn = (Button) findViewById(R.id.playlistbutton);
        mytenforBtn = (Button) findViewById(R.id.tenfor);
        mytenbackBtn = (Button) findViewById(R.id.tenback);
        myreplayBtn = (Button) findViewById(R.id.replay);
        mySendRequestBtn.setOnClickListener(myOnSendRequestClickHandler);
        myResumePauseRequestBtn.setOnClickListener(mySendPauseResumeClickHandler);
        myNextRequestBtn.setOnClickListener(mySendNextClickHandler);
        myPrevRequestBtn.setOnClickListener(mySendPrevClickHandler);
        myplaylistBtn.setOnClickListener(myOpenPlaylistActivityHandler);
        myaddplBtn.setOnClickListener(addplBtnHandler);
        mytenforBtn.setOnClickListener(tenforbtnHandler);
        mytenbackBtn.setOnClickListener(tenbackbtnHandler);
        myreplayBtn.setOnClickListener(replaybtnHandler);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_NOSENSOR);
    }

    public void startTimer() {
        //set a new Timer
        timer = new Timer();

        //initialize the TimerTask's job
        initializeTimerTask();

        //schedule the timer, after the first 5000ms the TimerTask will run every 10000ms
        timer.schedule(timerTask, 0, 100); //
    }

    public void stoptimertask() {
        //stop the timer, if it's not already null
        if (timer != null) {
            timer.cancel();
            timer = null;
        }
    }

    public void initializeTimerTask() {

        timerTask = new TimerTask() {
            public void run() {

                //use a handler to run a toast that shows the current timestamp
                handler.post(new Runnable() {
                    public void run() {
                        imgview.setRotation(s);
                        if(s==360.0){
                            s = 0;
                        }
                        s+=20.0;
                    }
                });
            }
        };
    }

    @Override
    public void onRestart(){
        super.onRestart();
        if(serverip != null) {
            mySendRequestBtn.performClick();
            mySendRequestBtn.performClick();
        }
    }

    @Override
    public void onDestroy()
    {
        // Stop listening to response messages.
        mySender.detachDuplexOutputChannel();
        f.delete();
        super.onDestroy();
    }

    private void openConnection() throws Exception
    {
        serverip = myMassageEditText.getText().toString();
        // Create sender sending MyRequest and as a response receiving MyResponse
        IDuplexTypedMessagesFactory aSenderFactory = new DuplexTypedMessagesFactory();
        mySender = aSenderFactory.createDuplexTypedMessageSender(MyResponse.class, MyRequest.class);

        // Subscribe to receive response messages.
        mySender.responseReceived().subscribe(myOnResponseHandler);

        // Create TCP messaging for the communication.
        // Note: 10.0.2.2 is a special alias to the loopback (127.0.0.1)
        //       on the development machine.
        IMessagingSystemFactory aMessaging = new TcpMessagingSystemFactory();
        IDuplexOutputChannel anOutputChannel = aMessaging.createDuplexOutputChannel("tcp://"+serverip+":"+portnum+"/");
        mySender.attachDuplexOutputChannel(anOutputChannel);

    }


    private EventHandler<TypedResponseReceivedEventArgs<MyResponse>> myOnResponseHandler
            = new EventHandler<TypedResponseReceivedEventArgs<MyResponse>>()
    {
        @Override
        public void onEvent(Object sender,
                            TypedResponseReceivedEventArgs<MyResponse> e)
        {
            onResponseReceived(sender, e);
        }
    };

    private void onResponseReceived(Object sender,
                                    final TypedResponseReceivedEventArgs<MyResponse> e)
    {

        myRefresh.post(new Runnable()
        {
            @Override
            public void run()
            {
                if(e.getResponseMessage().Length == 2){ //getmusicname
                        myMusicName.setText(e.getResponseMessage().Text);
                        musicname = e.getResponseMessage().Text;
                }
                else if (e.getResponseMessage().Length == 1){//get if added to playlist

                    Toast.makeText(getApplicationContext(),"Music successfully added to playlist",Toast.LENGTH_SHORT).show();

                }
                else if (e.getResponseMessage().Length == 0){//get if is not added to playlist

                    Toast.makeText(getApplicationContext(),"Music adding to playlist is unsuccessfull",Toast.LENGTH_SHORT).show();

                }
                else if (e.getResponseMessage().Length == 3){//get paused on startup
                            pauseclicktimes++;
                            myResumePauseRequestBtn.setBackgroundResource(R.drawable.play);
                            stoptimertask();
                }
                else if (e.getResponseMessage().Length == 4){//get playing on startup
                            myResumePauseRequestBtn.setBackgroundResource(R.drawable.play);
                            startTimer();
                } else if (e.getResponseMessage().Length == 5) {//get if program closed
                    disconnect();

                }
                else if(e.getResponseMessage().Length == 6){//no music in playlist
                    myMusicName.setText(e.getResponseMessage().Text);
                    myResumePauseRequestBtn.setEnabled(false);
                    myNextRequestBtn.setEnabled(false);
                    myPrevRequestBtn.setEnabled(false);
                }
                else if (e.getResponseMessage().Length == 7){

                    stoptimertask();
                    startTimer();
                    myResumePauseRequestBtn.setBackgroundResource(R.drawable.pause);

                }
                else if (e.getResponseMessage().Length == 8){

                    stoptimertask();
                    myResumePauseRequestBtn.setBackgroundResource(R.drawable.play);

                }
                else if (e.getResponseMessage().Length == 9){
                    writetoPlaylist(e.getResponseMessage().Text);
                }
            }
        });
    }

    private void disconnect(){
        Toast.makeText(getApplicationContext(),"Connection closed by server",Toast.LENGTH_SHORT).show();
        infoip.setText("Insert IP Address to Connect");
        myMassageEditText.setVisibility(View.VISIBLE);
        mySendRequestBtn.setBackgroundResource(R.drawable.connect);
        myMusicName.setText("Connect to a Server to See What it is Playing");
        stoptimertask();
        pauseclicktimes = 0;
        myResumePauseRequestBtn.setEnabled(false);
        myPrevRequestBtn.setEnabled(false);
        myNextRequestBtn.setEnabled(false);
        mySender.detachDuplexOutputChannel();
    }


    private void onSendRequest(String message)
    {
        // Create the request message.
        final MyRequest aRequestMsg = new MyRequest();
        aRequestMsg.Text = message;
        // Send the request message.
        try
        {
            mySender.sendRequestMessage(aRequestMsg);
        }
        catch (Exception err)
        {
            EneterTrace.error("Sending the message failed.", err);
        }

    }

    private OnClickListener myOnSendRequestClickHandler;

    {
        myOnSendRequestClickHandler = new OnClickListener() {
            @Override
            public void onClick(final View v) {
                Thread anOpenConnectionThread = new Thread(new Runnable() {
                    @Override
                    public void run() {
                        try {
                            openConnection();
                            onSendRequest("connected");

                        } catch (Exception err) {
                            infoip.setText("Insert IP Address to Connect");
                            myMassageEditText.setVisibility(View.VISIBLE);
                            mySendRequestBtn.setBackgroundResource(R.drawable.connect);
                            pauseclicktimes = 0;
                            myResumePauseRequestBtn.setEnabled(false);
                            myPrevRequestBtn.setEnabled(false);
                            myNextRequestBtn.setEnabled(false);
                            myMusicName.setText("Connect to a Server to See What it is Playing");
                            return;
                        }
                    }
                });
                if (connectclicktimes % 2 == 0) {
                    infoip.setText("Connected to : " + myMassageEditText.getText().toString());
                    myMassageEditText.setVisibility(View.INVISIBLE);
                    mySendRequestBtn.setBackgroundResource(R.drawable.disconnect);
                    anOpenConnectionThread.start();
                    myResumePauseRequestBtn.setEnabled(true);
                    myPrevRequestBtn.setEnabled(true);
                    myNextRequestBtn.setEnabled(true);
                } else {
                    infoip.setText("Insert IP Address to Connect");
                    myMassageEditText.setVisibility(View.VISIBLE);
                    mySendRequestBtn.setBackgroundResource(R.drawable.connect);
                    pauseclicktimes = 0;
                    onSendRequest("disconnect");
                    mySender.detachDuplexOutputChannel();
                    stoptimertask();
                    myResumePauseRequestBtn.setEnabled(false);
                    myPrevRequestBtn.setEnabled(false);
                    myNextRequestBtn.setEnabled(false);
                    myMusicName.setText("Connect to a Server to See What it is Playing");
                }
                connectclicktimes++;
            }

        };
    }

    private OnClickListener myOpenPlaylistActivityHandler = new OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            startPlaylistActivity();
        }
    };

    private OnClickListener mySendPauseResumeClickHandler = new OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            if(pauseclicktimes%2 == 1) {
                myResumePauseRequestBtn.setBackgroundResource(R.drawable.pause);
                startTimer();
            }
            else {
                myResumePauseRequestBtn.setBackgroundResource(R.drawable.play);
                stoptimertask();
            }
            pauseclicktimes++;
            onSendRequest("pause");
        }
    };

    private OnClickListener mySendNextClickHandler = new OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            onSendRequest("next");
            pauseclicktimes = 0;
            myResumePauseRequestBtn.setBackgroundResource(R.drawable.pause);
            stoptimertask();
            startTimer();
        }
    };

    private OnClickListener mySendPrevClickHandler = new OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            onSendRequest("prev");
            pauseclicktimes = 0;
            myResumePauseRequestBtn.setBackgroundResource(R.drawable.pause);
            stoptimertask();
            startTimer();
        }

    };

    private OnClickListener addplBtnHandler = new OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            onSendRequest("addplaylist");
        }

    };


    private OnClickListener  tenforbtnHandler = new OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            onSendRequest("tenfor");
        }

    };

    private OnClickListener  tenbackbtnHandler = new OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            onSendRequest("tenback");
        }

    };


    private OnClickListener  replaybtnHandler = new OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            if(replay == false){
                replay = true;
                onSendRequest("playagain");
                myreplayBtn.setBackgroundResource(R.drawable.doplay);
            }
            else{
                replay = false;
                onSendRequest("dontplayagain");
                myreplayBtn.setBackgroundResource(R.drawable.dontplay);
            }
        }

    };

    private void startPlaylistActivity(){
        mySender.detachDuplexOutputChannel();
        Intent intent = new Intent(this, playlist.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        intent.putExtra(EXTRA_MESSAGE, serverip);
        startActivityForResult(intent,1);
    }

    @Override
    public boolean dispatchKeyEvent(KeyEvent event) {
        int action = event.getAction();
        int keyCode = event.getKeyCode();
        switch (keyCode) {
            case KeyEvent.KEYCODE_VOLUME_UP:
                if (action == KeyEvent.ACTION_DOWN) {
                    onSendRequest("volumeup");
                }
                return true;
            case KeyEvent.KEYCODE_VOLUME_DOWN:
                if (action == KeyEvent.ACTION_DOWN) {
                    onSendRequest("volumedown");
                }
                return true;
            default:
                return super.dispatchKeyEvent(event);
        }
    }

    protected void writetoPlaylist(String music){
        try {
            BufferedWriter bw = new BufferedWriter(new FileWriter(f.getAbsolutePath(),true));
            bw.write(music+"\n");
            bw.flush();
            bw.close();
        } catch (IOException ioe)
        {ioe.printStackTrace();}
    }

}

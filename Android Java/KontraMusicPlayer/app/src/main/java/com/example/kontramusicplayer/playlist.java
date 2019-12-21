package com.example.kontramusicplayer;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.ScrollView;
import android.widget.TextView;
import android.view.View.OnClickListener;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Timer;
import java.util.TimerTask;

import eneter.messaging.diagnostic.EneterTrace;
import eneter.messaging.endpoints.typedmessages.DuplexTypedMessagesFactory;
import eneter.messaging.endpoints.typedmessages.IDuplexTypedMessageSender;
import eneter.messaging.endpoints.typedmessages.IDuplexTypedMessagesFactory;
import eneter.messaging.endpoints.typedmessages.TypedResponseReceivedEventArgs;
import eneter.messaging.messagingsystems.messagingsystembase.IDuplexOutputChannel;
import eneter.messaging.messagingsystems.messagingsystembase.IMessagingSystemFactory;
import eneter.messaging.messagingsystems.tcpmessagingsystem.TcpMessagingSystemFactory;
import eneter.net.system.EventHandler;



public class playlist extends Activity {

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
    final Handler handler = new Handler();
    Timer timer;
    TimerTask timerTask;
    Button musicoptions;
    ScrollView sv;
    TextView musicnameplaylist;
    Button goback;
    ListView lw;
    File f ;
    ImageView imgview;
    float s;
    String serverip;
    String portnum = "8060";
    int pauseclicktimes = 0;
    private IDuplexTypedMessageSender<MyResponse, MyRequest> mySender;
    private Handler myRefresh = new Handler();

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_playlist);
        musicoptions = new Button(this);
        musicnameplaylist = findViewById(R.id.musicName);
        goback = findViewById(R.id.button);
        goback.setOnClickListener(myGoBack);
        lw = findViewById(R.id.playlistview);
        ArrayAdapter<String> listadapter = new ArrayAdapter<String>(this,android.R.layout.activity_list_item);
        f = new File("/data/data/com.example.kontramusicplayer/files/musics.mpl");
        readfromPlaylist();
        Intent intent = getIntent();
        serverip = intent.getStringExtra(MainActivity.EXTRA_MESSAGE);
        imgview = (ImageView)findViewById(R.id.imageView5);
        if(serverip != null) {
            startTimer();
            connection();
        }
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


    private View.OnClickListener myGoBack = new View.OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            goBack();
        }
    };

    private void connection(){
        Thread anOpenConnectionThread = new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    openConnection();
                    onSendRequest("connected");
                } catch (Exception err) {
                    return;
                }
            }
        });
        anOpenConnectionThread.start();
    }


    private void openConnection() throws Exception
    {

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

    public void goBack(){
        mySender.detachDuplexOutputChannel();
        finish();
    }

    private void readfromPlaylist(){

        String music = null;
        ArrayList<String> musics = new ArrayList<String>();
        try{
        BufferedReader reader = new BufferedReader(new FileReader(f.getAbsolutePath()));
        while((music = reader.readLine()) != null){
            musics.add(music);
        }
            TextView t = new TextView(this);
            ArrayAdapter<String> myadapter=new ArrayAdapter<String>
                    (this, android.R.layout.simple_list_item_1, android.R.id.text1, musics);
            lw.setAdapter(myadapter);
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }

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
            public void run(){

                if(e.getResponseMessage().Length == 9){

                }

            }
        });

    }
}

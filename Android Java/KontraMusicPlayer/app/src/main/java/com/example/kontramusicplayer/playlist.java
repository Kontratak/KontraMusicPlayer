package com.example.kontramusicplayer;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.ScrollView;
import android.widget.TextView;

import java.util.ArrayList;

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
    Button musicoptions;
    ScrollView sv;
    TextView musicnameplaylist;
    Button goback;
    ListView list;
    ImageView imgview;
    String serverip;
    String portnum = "8060";
    ArrayList<String> playlist = new ArrayList<String>();
    private IDuplexTypedMessageSender<MyResponse, MyRequest> mySender;
    private Handler myRefresh = new Handler();
    private Context mContext;
    int count = 0;
    int length = -1;
    int index;
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_playlist);
        musicoptions = new Button(this);
        musicnameplaylist = findViewById(R.id.musicName);
        goback = findViewById(R.id.button);
        goback.setOnClickListener(myGoBack);
        list =(ListView) findViewById(R.id.playlistview);
        Intent intent = getIntent();
        String serverip = intent.getExtras().getString("serverip");
        this.serverip = serverip;
        imgview = (ImageView)findViewById(R.id.imageView5);
        mContext = this;
        list.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                onSendRequest("getindex"+Integer.toString(position));
            }
        });
        if(serverip != null) {
            connection();
        }
    }

    private View.OnClickListener myGoBack = new View.OnClickListener()
    {
        @Override
        public void onClick(View v)
        {
            onSendRequest("playlistclosed");
            mySender.detachDuplexOutputChannel();
            finish();
        }
    };

    private void connection(){
        Thread anOpenConnectionThread = new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    openConnection();
                    onSendRequest("playlistopen");
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
                if(e.getResponseMessage().Length == 11){
                    length = Integer.parseInt(e.getResponseMessage().Text);
                }
                else if(e.getResponseMessage().Length == 9) {
                    playlist.add(e.getResponseMessage().Text);
                    count++;
                    if(length == count){
                        addtoAdaptor();
                    }
                }
                else if(e.getResponseMessage().Length == 12){
                    //TODO
                }
            }
        });
    }

    private void addtoAdaptor(){
        ArrayAdapter<String> playlistAdaptor=new ArrayAdapter<>(mContext, android.R.layout.simple_list_item_1, android.R.id.text1, playlist);
        list.setAdapter(playlistAdaptor);
    }

}

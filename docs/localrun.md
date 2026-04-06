How to Use
1. Launch Chrome with remote debugging:
    ```  
    /Applications/Google\ Chrome.app/Contents/MacOS/Google\ Chrome \
     --remote-debugging-port=9222 \
     --user-data-dir=$HOME/.chrome-automation-profile
   ```
2. Log into higgsfield.ai in that Chrome window
3. Rebuild and run: docker-compose -f docker/docker-compose.yml up -d --build
4. Open http://localhost:3000/automate — click "Connect to Chrome"
5. Choose a mode (Image, Video, or Cinema Studio), enter a prompt, and click Generate. The automation will control your Chrome browser and show results in the UI.
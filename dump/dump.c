#include <stdio.h>
#include <windows.h>
#include <sys/types.h>
#include <string.h>

#define PORT        7878
#define HOST        "localhost"
#define BUFFER_SIZE 1024

void cleanup_and_exit(int ret)
{
  WSACleanup();
  exit(ret);
}

int main(int argc, char **argv)
{
  char hostname[100];
	SOCKET	sd;
	struct sockaddr_in sin;
	struct sockaddr_in pin;
	struct hostent *hp;
  char buffer[BUFFER_SIZE];
  WSADATA wsaData;
  
  if (WSAStartup(MAKEWORD(2,0), &wsaData) != 0) {
    fprintf(stderr, "WSAStartup failed\n");
    cleanup_and_exit(1);
  }
  
  strcpy(hostname,HOST);
  if (argc>2) { 
    strcpy(hostname,argv[2]); 
  }
    
	/* go find out about the desired host machine */
	if ((hp = gethostbyname(hostname)) == 0) {
		perror("gethostbyname");
		cleanup_and_exit(1);
	}

	/* fill in the socket structure with host information */
	memset(&pin, 0, sizeof(pin));
	pin.sin_family = AF_INET;
	pin.sin_addr.s_addr = ((struct in_addr *)(hp->h_addr))->s_addr;
	pin.sin_port = htons(PORT);

	/* grab an Internet domain socket */
	if ((sd = socket(AF_INET, SOCK_STREAM, 0)) == -1) {
		perror("socket");
		cleanup_and_exit(1);
	}

	/* connect to PORT on HOST */
	if (connect(sd,(struct sockaddr *)  &pin, sizeof(pin)) == -1) {
		perror("connect");
		cleanup_and_exit(1);
	}

    /* wait for a message to come back from the server */
    while (1) {
      int count = recv(sd, buffer, BUFFER_SIZE, 0);
      if (count == -1) {
        closesocket(sd);
        perror("recv");
        cleanup_and_exit(1);
      }
      if (count == 0)
        break;
      fwrite(buffer, 1, count, stdout);
  }

	closesocket(sd);
  WSACleanup();
	
  return 0;
}

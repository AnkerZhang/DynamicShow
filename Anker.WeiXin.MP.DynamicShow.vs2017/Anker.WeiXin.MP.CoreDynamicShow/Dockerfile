FROM microsoft/dotnet
 
WORKDIR /app
 
COPY . /app

RUN apt-get update
RUN apt-get install libgdiplus --assume-yes
RUN cd /usr/lib
RUN ln -s libgdiplus.so gdiplus.dll

 
CMD ["dotnet","run"]
 
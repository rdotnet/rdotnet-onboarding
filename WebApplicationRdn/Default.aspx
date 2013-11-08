<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplicationRdn.RenginePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">


h3 {
    font-size: 1.2em;
}

h1, h2, h3,
h4, h5, h6 {
    color: #000;
    margin-bottom: 0;
    padding-bottom: 0;
}

    input {
        width: 90%;
    }

    
    
    input, textarea {
        border: 1px solid #e2e2e2;
        background: #fff;
        color: #333;
        font-size: 1.2em;
        margin: 5px 0 6px 0;
        padding: 5px;
        }

    input[type="submit"],
    input[type="button"],
    button {
        background-color: #d3dce0;
        border: 1px solid #787878;
        cursor: pointer;
        font-size: 1.2em;
        font-weight: 600;
        padding: 7px;
        margin-right: 8px;
        width: auto;
    }

    ol.round {
        list-style-type: none;
        padding-left: 0;
    }

        ol.round {
    list-style-type: none;
    padding-left: 0;
}

            ol.round li.zero,
            ol.round li.one,
            ol.round li.two,
            ol.round li.three,
            ol.round li.four,
            ol.round li.five,
            ol.round li.six,
            ol.round li.seven,
            ol.round li.eight,
            ol.round li.nine {
                background: none;
            }

        ol.round li.one {
            background: url('http://localhost:53686/Images/orderedList1.png') no-repeat;
        }

        ol.round li {
            padding-left: 10px;
            margin: 25px 0;
        }

            ol.round li {
        margin: 25px 0;
        padding-left: 45px;
    }

        h5, h6 {
    font-size: 1em;
}

    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        A minimal ASP.NET page accessing R via R.NET<br />
        Written by an ASP.NET newbie - be forgiving...<br />
        <br />
        Set the CStackLimit to -1:&nbsp;
        <asp:Button ID="btnSetCStack" runat="server" OnClick="btnSetCStack_Click" Text="CStackLimit = -1" />
        </div>
    <h3>REPL</h3>
        <p>&nbsp;Enter R statement input </p>
        <p>
            <asp:TextBox ID="tbStatement" runat="server" Height="213px" TextMode="MultiLine" Width="896px"></asp:TextBox>
            <asp:Button ID="btSubmitStatement" runat="server" OnClick="btSubmitStatement_Click" Text="Submit" />
        </p>
        <p>&nbsp;(DOES NOT WORK) source file:<asp:FileUpload ID="fuRscript" runat="server" Enabled="False" />
            <asp:Button ID="btSubmitScript" runat="server" OnClick="btSubmitScript_Click" Text="Submit" Enabled="False" />
&nbsp;&nbsp;</p>
        <p>&nbsp; </p>
        <p>Result:</p>
        <p>
            <asp:Button ID="btnClear" runat="server" OnClick="btnClear_Click" Text="Clear" />
        </p>
        <p>&nbsp;<asp:TextBox ID="tbResult" runat="server" Height="228px" TextMode="MultiLine" Width="540px"></asp:TextBox>
        </p>
    </form>
</body>
</html>

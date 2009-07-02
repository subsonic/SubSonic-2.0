<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Theme="Default" MasterPageFile="~/res/MasterPage.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphLeft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphRight" Runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphFull" Runat="Server">
<h3>Welcome to SubSonic 2.1!</h3>
    <p>
        What is SubSonic? Well, simply put it's an effort by a group of .NET geeks
        to bring Ruby On Rails simplicity to .NET. We're starting with the Database and
        are slowly moving our way outward to web applications in general. This is done with
        a combination of OR/Mapping, code generation, Unit Testing, and adherence to convention
        (over configuration).</p>
        Don't take this the wrong way - we're NOT OR/M nuts. We fully support and in fact 
        want you to use SPs and Views - but when you need to, not because you have to. The choice
        is yours, SubSonic will wrap these things for you and off you go!
    <p>
        A lot has been made of Rails, and much of the hype is deserved. However the fun
        in Rails is not necessarily Rails itself - it's the joy if being able to do the
        work you love without the tedium of doing the same thing over and over (like creating
        a Data Access Layer).</p>
    <p>
        It's our goal to bring this simplicity to you. We're already using it on the Commerce
        Starter Kit for ASP.NET 2.0. It's all yours - free!</p>
        
 <h3>What's New in 2.1?</h3>
    <p>
        With version 2.1 we've addressed some major requests from our community, and tried
        to add in functionality to make your day that much easier. Here's what's new:</p>
    <p>
        <strong>New Query Engine</strong> - The new Query Engine provides even more powerful programmatic querying capabilties
        while improving syntax clarity and readability.
    </p>
    <p>
        <strong>SubStage</strong> - Our new configuration and management utility makes configuring your application for SubSonic a breeze.
    </p>
        <p>
        <strong>Migrations</strong> - The SubSonic.Migrations namespace takes the pain out of migrating your applications between database versions.
    </p>
            <p>
        <strong>Documentation</strong> - While documentation is a task that's never done, we've made great strides in improving code commenting, and the SubSonic installer now includes a robust help file.
    </p>
    
    <p>
        <strong>Speed and Stability</strong> - We've done a lot of tuning and knock down a lot of bugs, making 2.1 the fastest and most solid SubSonic release yet.
    </p>

    <p>
        &nbsp;</p>
</asp:Content>
/*
 * SubSonic - http://subsonicproject.com
 * 
 * The contents of this file are subject to the Mozilla Public
 * License Version 1.1 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of
 * the License at http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or
 * implied. See the License for the specific language governing
 * rights and limitations under the License.
*/
using System;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="ItemType">The type of the tem type.</typeparam>
    /// <typeparam name="ListType">The type of the ist type.</typeparam>
    [Serializable]
    public class RepositoryList<ItemType, ListType> : AbstractList<ItemType, ListType>
        where ItemType : RepositoryRecord<ItemType>, new()
        where ListType : AbstractList<ItemType, ListType>, new() {}
}
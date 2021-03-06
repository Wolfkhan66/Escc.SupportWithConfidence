﻿using System;
using System.Collections.Generic;
using System.Data;
using EsccWebTeam.SupportWithConfidence.Controls;

namespace Escc.SupportWithConfidence.Controls
{
    /// <summary>
    /// Category Mapper takes a datatable of Categories via the constructor and returns a collection of Category objects 
    /// structured as a family tree via the Categories property.
    /// </summary>
    public class CategoryMapper
    {
        // Used to hold the top level categoeries who parentid = null
        private IList<Category> _topCategories = new List<Category>();


        /// <summary>
        /// The constructor takes a datatable and loops over ever row, creating a collection of new category objects. Each category object
        /// after creation is checked to see if exists as a parent or a child category. The outcome of this loop will produce a collection of 
        /// Category objects structured as a family tree. Use the Categories property to access the category collection.
        /// </summary>
        /// <param name="dbcategories">DataSet</param>
        public CategoryMapper(DataSet dbcategories)
        {
            if (dbcategories == null) return;
            foreach (DataRow dbcategory in dbcategories.Tables[0].Rows)
            {
                var category = new Category
                    {
                        Id = Convert.ToInt16(dbcategory["Id"]),
                        Code = dbcategory["Code"].ToString(),
                        Description = dbcategory["Description"].ToString(),
                        ParentId =
                            dbcategory["ParentId"] == DBNull.Value
                                ? 0
                                : Convert.ToInt16(dbcategory["ParentId"]),
                        Depth = Convert.ToInt16(dbcategory["Depth"]),
                        ProviderTypeId = ProviderType.SupportwithConfidence,
                        IsActive = Convert.ToBoolean(dbcategory["IsActive"]),
                        Sequence = Convert.ToInt32(dbcategory["Sequence"])
                    };


                if (dbcategory["ParentId"] == DBNull.Value)
                {
                    _topCategories.Add(category);
                }
                else
                {
                    var parentId = Convert.ToInt16(dbcategory["ParentId"]);
                    foreach (var topCategory in _topCategories)
                    {
                        var parentCategory = topCategory.FindCategory(parentId);
                        if (parentCategory == null) continue;
                        parentCategory.Categories.Add(category);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Returns a collection of Category objects ordered as a family tree.
        /// </summary>
        public IList<Category> Categories
        {
            get { return _topCategories; }
            set { _topCategories = value; }
        }
    }
}